# Arquitetura de Microsservicos - Ganho de Capital MVP2

## 1. Visao Geral e Diretrizes

Esta proposta descreve uma arquitetura de microsservicos assincrona, projetada para performance, escalabilidade e resiliencia na AWS. O design prioriza o desacoplamento de servicos e a tolerancia a falhas, utilizando um fluxo de processamento baseado em filas para garantir a integridade das operacoes.

**Principios Chave:**
* **Desacoplamento:** Servicos se comunicam de forma assincrona via filas (SQS), minimizando dependencias diretas e permitindo evolucao independente.
* **Escalabilidade:** A arquitetura utiliza componentes serverless e auto-scaling (ECS Fargate, Lambda) para se adaptar dinamicamente a carga, otimizando custos e performance.
* **Resiliencia:** O sistema e desenhado para ser tolerante a falhas, com retentativas automaticas (retries), filas de mensagens mortas (DLQ) e alta disponibilidade na camada de dados (Multi-AZ).
* **Observabilidade:** Monitoramento centralizado com CloudWatch e tracing distribuido com X-Ray para garantir visibilidade completa do fluxo de ponta a ponta.

## 2. Diagrama da Arquitetura
Inserida imagem do diagrama no repositorio.

## 3. Fluxo de Processamento Assincrono

1.  **Requisicao:** O cliente envia a requisicao para o **Tax Service** atraves de um API Gateway e Load Balancer.
2.  **Validacao e Enfileiramento:** O **Tax Service** realiza uma validacao inicial, enfileira a operacao na fila SQS (`tax-processing-queue`) e retorna imediatamente uma resposta de aceite (`202 Accepted`) com o status "Em processamento".
3.  **Processamento:** Uma funcao Lambda e acionada pela mensagem na fila SQS. Ela assume a responsabilidade de orquestrar o processo:
    a.  Enriquece os dados do cliente chamando o **Client Service**.
    b.  Executa a logica de negocio complexa para o calculo do imposto.
    c.  Persiste o resultado final e o status no banco de dados **SQL Server**.
4.  **Consulta:** A interface do usuario, atraves do **BFF (Backend for Frontend)**, consulta o status da operacao diretamente no banco de dados.

## 4. Decisoes Chave de Tecnologia

#### **Por que SQL Server no RDS?**
Para um ambiente corporativo como o do Itau, a escolha do SQL Server e estrategica.
* **Ecossistema .NET:** Oferece integracao nativa e otimizada com o ecossistema .NET e o Entity Framework, reduzindo a friccao no desenvolvimento.
* **Suporte e Licenciamento Enterprise:** O banco ja possui um ecossistema de suporte, ferramentas de administracao (SSMS) e, frequentemente, acordos de licenciamento corporativo pre-existentes, facilitando a gestao e a conformidade.
* **Maturidade e Seguranca:** E uma plataforma de dados extremamente madura, com recursos de seguranca robustos e adequados as exigencias do setor financeiro.

#### **Por que SQS + Lambda para Processamento?**
Essa combinacao forma o nucleo do nosso design assincrono.
* **Desacoplamento e Resiliencia:** O SQS atua como um *buffer*, absorvendo picos de requisicoes e garantindo que nenhuma operacao seja perdida caso o processador esteja indisponivel. A logica de *retry* e DLQ e tratada nativamente.
* **Custo-Eficiencia e Escalabilidade:** O Lambda escala de zero a milhares de execucoes concorrentes sem gerenciamento de infraestrutura e com um modelo de custo *pay-per-use*, ideal para cargas de trabalho variaveis.

#### **Por que ECS Fargate para os Servicos?**
* **Simplicidade Operacional:** Fargate abstrai o gerenciamento de servidores, permitindo que a equipe foque no desenvolvimento da aplicacao em conteineres, enquanto a AWS cuida da escalabilidade e provisionamento da infraestrutura subjacente.
* **Isolamento e Seguranca:** Cada tarefa roda em seu proprio ambiente isolado, alinhado com as boas praticas de seguranca para microsservicos.

## 5. Reprocessamento e Auditoria

* **Mecanismo de Reprocessamento:** Uma operacao que falhou (ou que precisa ser corrigida) pode ser reenfileirada em uma fila dedicada (`tax-reprocessing-queue`) atraves de um endpoint no BFF. Isso permite a re-execucao do calculo sem a necessidade de uma nova submissao completa.
* **Auditoria:** Todas as alteracoes de estado e acoes de reprocessamento sao registradas em uma tabela de auditoria (`audit_log`) para garantir a rastreabilidade e conformidade, um requisito fundamental no setor financeiro.       