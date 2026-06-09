# Fiap Cloud Games (FCG) - Infraestrutura e Orquestração

> Projeto da Fase 2 do Tech Challenge da pós-graduação da FIAP.

Este repositório atua como o **Orquestrador de Infraestrutura** do projeto Fiap Cloud Games. O sistema original (monolítico) foi re-arquitetado para um formato de **Microsserviços Orientados a Eventos**, utilizando RabbitMQ (via MassTransit) e contêineres Docker/Kubernetes.

## Microsserviços

O código-fonte de cada microsserviço foi isolado em seu próprio repositório para garantir escalabilidade, independência de deploy e coesão de domínio. Acesse-os nos links abaixo:

- 👤 **[UsersAPI](https://github.com/pollymsr/UsersAPI)**: Responsável por Autenticação (JWT) e gestão de usuários.
- 🎮 **[CatalogAPI](https://github.com/pollymsr/CatalogAPI)**: Responsável pelo Catálogo de Jogos, Promoções, Biblioteca do Usuário e início do fluxo de Checkout.
- 💳 **[PaymentsAPI](https://github.com/pollymsr/PaymentsAPI)**: Worker isolado (consumer) que processa pagamentos assíncronos.
- 📧 **[NotificationsAPI](https://github.com/pollymsr/NotificationsAPI)**: Worker responsável por disparar notificações e e-mails aos usuários baseando-se em eventos do sistema.

## Como executar via Docker Compose

Para subir o sistema inteiro de forma integrada na sua máquina (todos os 4 serviços + o RabbitMQ):

1. Certifique-se de ter feito o `git clone` dos 4 microsserviços na mesma pasta raiz onde este repositório `FiapCloudGames-Infra` está (um ao lado do outro).
2. Na raiz deste repositório, rode:
```bash
docker-compose build
docker-compose up -d
```

3. Acesse os endpoints pelo Swagger gerado em cada API (listados nas portas configuradas).

## Como executar no Kubernetes

Na pasta `k8s/` estão os manifestos (Deployments, Services e ConfigMaps) necessários para orquestrar o sistema completo em um cluster Kubernetes.

```bash
kubectl apply -f ./k8s
```