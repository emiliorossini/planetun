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

- - 📄 [Ver implementação](./questao2.cs)

---
