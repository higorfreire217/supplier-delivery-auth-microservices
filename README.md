# Instruções para Executar o Projeto com Docker Compose

## Pré-requisitos

- Docker instalado
- Docker Compose instalado

## Como executar

1. No terminal, navegue até o diretório raiz do projeto.
2. Execute o comando abaixo para iniciar os serviços:
    ```bash
    docker-compose up
    ```
3. Para rodar em segundo plano:
    ```bash
    docker-compose up -d
    ```
4. Para parar os serviços:
    ```bash
    docker-compose down
    ```

## Estrutura de Diretórios

- `architecture/`  
  Arquivos de definição da arquitetura do projeto, incluindo o `Makefile` e descrições do workspace.

- `features/`  
  Cenários de testes BDD (Behavior Driven Development) para autenticação e fornecedores, além dos scripts de passos.

- `services/`  
  Microserviços do projeto, organizados em `auth` e `supplier`, cada um com seu código, Dockerfile, README e testes.

- `docker-compose.yml`  
  Arquivo principal para orquestração dos containers.

- `supplier-delivery-auth-microservices.sln`  
  Solução principal do projeto.

## Observações

- Consulte os arquivos de configuração para ajustar variáveis de ambiente conforme necessário.
- Logs dos serviços podem ser visualizados diretamente no terminal ou via `docker logs <nome-do-container>`.
- Para mais detalhes sobre cada microserviço, veja a documentação em `docs/`.

# Instruções para Executar o Projeto com Docker Compose

## Executando com Dev Container (VS Code)

Você pode executar o projeto em um ambiente de desenvolvimento isolado usando Dev Containers no VS Code:

1. Certifique-se de ter o Docker e o VS Code instalados, além da extensão [Dev Containers](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers).
2. Crie o diretório `.devcontainer` na raiz do projeto:
    ```sh
    mkdir .devcontainer
    ```
3. Dentro do diretório `.devcontainer`, crie o arquivo `devcontainer.json` com o seguinte conteúdo básico:
    ```json
    {
      "name": "Supplier Delivery Auth DevContainer",
      "build": {
         "dockerfile": "Dockerfile"
      },
      "settings": {},
      "extensions": [
         "ms-dotnettools.csharp",
         "ms-python.python"
      ],
      "forwardPorts": [5000, 5001],
      "postCreateCommand": "dotnet restore"
    }
    ```
4. Crie o arquivo `Dockerfile` em `.devcontainer` com um exemplo de configuração:
    ```Dockerfile
    FROM mcr.microsoft.com/dotnet/sdk:8.0
    # Instala Python e dependências BDD
    RUN apt-get update && \
        apt-get install -y python3 python3-pip python3-behave python3-requests
    # Copia os arquivos da aplicação
    COPY . /workspaces/supplier-delivery-auth-microservices
    WORKDIR /workspaces/supplier-delivery-auth-microservices
    # Instala pacotes dos projetos dotnet
    RUN dotnet restore supplier-delivery-auth-microservices.sln
    ```
5. Abra o diretório do projeto no VS Code.
6. Quando solicitado, selecione **"Reopen in Container"** para inicializar o ambiente de desenvolvimento.
7. O VS Code irá construir o container conforme definido em `.devcontainer/devcontainer.json` e instalar as dependências automaticamente.
8. Após o ambiente estar pronto, utilize o terminal integrado para rodar comandos dotnet, python ou docker conforme necessário.

> **Observação:** O arquivo `devcontainer.json` e o Dockerfile acima são exemplos básicos e podem ser ajustados conforme as necessidades do seu projeto.
