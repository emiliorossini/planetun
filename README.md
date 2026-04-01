# Teste Técnico - Planetun

## Questão 1 — Domínio de Desembolso Financeiro

### Decisões de design

- **`Solicitar()` como factory method estático** — garante que o `Desembolso` só pode ser criado já com o evento emitido e status correto, sem expor construtor público
- **`Valor` com `private set`** — atende o invariante (b): após criado, o valor não pode ser alterado externamente, nem após aprovação
- **`Pagar()` valida `Status == Aprovado`** — atende o invariante (a): só desembolsos aprovados podem ser pagos, lançando exceção caso contrário
- **Eventos emitidos dentro do próprio agregado** — o `Desembolso` é responsável por emitir seus próprios eventos sem depender de infraestrutura externa
- **`StatusDesembolso` como enum** — deixa o fluxo de estados explícito e evita uso de strings ou magic numbers

- 📄 [Ver implementação](./questao1.cs)

---

## Questão 2 — Webhook Handler com Segurança

### Decisões de design

- **HMAC-SHA256 via `X-Signature`** — valida que o payload não foi adulterado, usando `CryptographicOperations.FixedTimeEquals` para evitar timing attacks
- **Proteção contra replay attack via `X-Timestamp`** — rejeita requisições com timestamp superior a 5 minutos, impedindo reuso de requisições interceptadas
- **Idempotência via `X-Request-Id`** — armazena IDs já processados em um `HashSet`, garantindo que a mesma requisição não seja processada duas vezes
- **Retorno 200 imediato com `Task.Run`** — o processamento é feito de forma assíncrona via fila interna, evitando timeout no caller

- 📄 [Ver implementação](./questao2.cs)

---

## Questão 3 — Pipeline CI/CD no Azure DevOps

### Decisões de design

- **4 stages separados** — Build, CodeQuality, Docker e Deploy, cada um com responsabilidade única e dependência explícita entre eles
- **Variáveis por ambiente** — `dev` e `prod` são separados automaticamente pela branch (`develop` = dev, `main` = prod)
- **SonarCloud com cobertura** — o relatório OpenCover gerado nos testes é enviado ao SonarCloud para análise de qualidade
- **Rolling update no AKS** — garante zero downtime no deploy, promovendo apenas após health check bem-sucedido

- - 📄 [Ver implementação](./questao3.yaml)

---

## Questão 4 — Dockerfile Multi-stage e Deployment K8s

### Decisões de design

- **Multi-stage build** — stage `build` usa a SDK completa, stage `runtime` usa apenas o ASP.NET runtime, resultando em imagem final menor
- **Non-root user** — criação de usuário `appuser` sem privilégios de root, seguindo boas práticas de segurança em containers
- **3 replicas** — garante alta disponibilidade e tolerância a falhas
- **Liveness e Readiness probes** — liveness reinicia o container se travar, readiness remove do load balancer se não estiver pronto
- **Resource limits** — evita que um container consuma recursos excessivos afetando outros pods
- **ConfigMap + Secret** — variáveis de configuração separadas de credenciais sensíveis

- - 📄 [Ver implementação Dockerfile](./questao4.dockerfile)
- - 📄 [Ver implementação deployment.yaml:](./questao4.yaml)

---
