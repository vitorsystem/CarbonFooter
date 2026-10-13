# Projeto - Cidades ESG Inteligentes (CarbonFooter)

O CarbonFooter é uma API RESTful desenvolvida em C# (.NET 10) voltada para o monitoramento de sustentabilidade e governança ambiental corporativa (ESG). A aplicação permite registrar o consumo energético (kWh) por departamento e calcula automaticamente a pegada de carbono correspondente (em kg de CO²), persistindo os registros em um banco de dados NoSQL (MongoDB).

Este projeto integra práticas completas de DevOps, contemplando containerização Multi-Stage, orquestração local com Docker Compose e um pipeline automatizado de CI/CD via GitHub Actions com entrega contínua em ambientes isolados de Staging e Produção na nuvem Microsoft Azure.

---

## Como executar localmente com Docker

### Pré-requisitos
- [Docker](https://www.docker.com/) e Docker Compose instalados na máquina.
- Git instalado.

### Passo a passo
1. Clone o repositório:
   ```
   git clone https://github.com/vitorsystem/CarbonFooter.git
   cd CarbonFooter
   ```

2. Configure as variáveis de ambiente:
   Crie um arquivo `.env` na raiz do projeto copiando o modelo `.env.example`:
   ```
   cp .env.example .env
   ```
   Conteúdo padrão do `.env`:*
   ```
   DB_USER=admin
   DB_PASSWORD=senha_secreta_123
   DB_NAME=carbonfooterdb
   ```

3. Suba os contêineres (API + MongoDB):
   ```
   docker-compose up -d --build
   ```

4. Acesse e teste a aplicação:
   - Documentação interativa (Swagger UI): [http://localhost:8080/swagger](http://localhost:8080/swagger)
   - Endpoint base da API: `http://localhost:8080/api/esg`

5. Para encerrar os serviços:
   ```
   docker-compose down
   ```

---

## Pipeline CI/CD

O pipeline de Integração e Entrega Contínua foi implementado utilizando o GitHub Actions (arquivo `.github/workflows/pipeline.yml`) e é acionado automaticamente a cada `push` ou `pull_request` na branch `main`.

### Estrutura e Etapas do Pipeline:
1. Etapa 1 — Build e Testes Automatizados (`build-and-test`):
   - Provisiona um runner `ubuntu-latest` e configura o SDK do .NET 10.
   - Restaura os pacotes NuGet (`dotnet restore`) e compila a solução em modo `Release` (`dotnet build`).
   - Executa a suíte de testes unitários automatizados (`dotnet test`) com xUnit** e Moq, validando a regra de negócio de cálculo automático da emissão de CO² (fator `0.085 kg/kWh`). Se qualquer teste falhar, o pipeline é interrompido imediatamente.

2. Etapa 2 — Build da Imagem e Deploy em Staging (`deploy-staging`):
   - Depende da aprovação da etapa de testes (`needs: build-and-test`).
   - Autentica na conta da Microsoft Azure utilizando um Service Principal armazenado de forma segura no GitHub Secrets (`AZURE_CREDENTIALS`).
   - Realiza login no Azure Container Registry (ACR) (`acrcarbonfooterjoao.azurecr.io`), constrói a imagem Docker marcada com o hash único do commit (`${{ github.sha }}`) para garantir rastreabilidade e envia a imagem via `docker push`.
   - Atualiza o serviço Azure Web App for Containers de homologação (`carbonfooter-staging-joao`) para rodar a nova imagem.

3. Etapa 3 — Promoção para Produção (`deploy-production`):
   - Depende do sucesso completo do deploy em Staging (`needs: deploy-staging`).
   - Promove exatamente a mesma imagem Docker já validada (`${{ github.sha }}`) para o ambiente de Produção (`carbonfooter-prod-joao`), garantindo paridade entre os ambientes.

---

## Containerização

A aplicação foi containerizada seguindo as melhores práticas de segurança, performance e isolamento de ambiente:

### 1. Estratégia Multi-Stage Build (`Dockerfile`)
- Estágio de Build (`mcr.microsoft.com/dotnet/sdk:10.0`): Restaura dependências em camada separada para aproveitar o cache do Docker e compila os binários otimizados via `dotnet publish`.
- Estágio de Runtime (`mcr.microsoft.com/dotnet/aspnet:10.0`): Copia apenas os artefatos publicados para uma imagem enxuta que não contém o SDK de desenvolvimento, reduzindo drasticamente o tamanho final da imagem e a superfície de ataque.
- Uso de `.dockerignore`: Impede que pastas locais de compilação (`bin/`, `obj/`, `.vs/`) e arquivos sensíveis (`.env`) sejam copiados para dentro do contexto de build do contêiner Linux.

### 2. Orquestração Local (`docker-compose.yml`)
- Rede dedicada (`carbon-network`): Isola a comunicação interna entre a API e o banco de dados via DNS interno do Docker (a API se conecta ao host `mongodb` na porta `27017`).
- Volume persistente (`mongodb_data`): Garante que os documentos salvos no MongoDB não sejam perdidos quando os contêineres são reiniciados.
- Variáveis de ambiente: Injetadas dinamicamente via arquivo `.env` utilizando a convenção `MongoDbSettings__ConnectionString` do ASP.NET Core.

---

## Tecnologias utilizadas

| Categoria | Tecnologia / Ferramenta |
|---|---|
| **Backend & API** | C# / .NET 10, ASP.NET Core Web API, Swashbuckle (Swagger/OpenAPI) |
| **Banco de Dados** | MongoDB (Driver Oficial `MongoDB.Driver` / MongoDB Atlas na nuvem) |
| **Testes Automatizados** | xUnit, Moq |
| **Containerização & Orquestração** | Docker (Multi-Stage Build), Docker Compose |
| **CI/CD** | GitHub Actions |
| **Cloud & Infraestrutura** | Microsoft Azure (Resource Group, Azure Container Registry, Linux App Service Plan B1, Web App for Containers) |