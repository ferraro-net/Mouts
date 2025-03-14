# Projeto Giga House

Bem-vindo ao repositório do projeto Giga House! 
Este projeto consiste em uma API desenvolvida em .NET Core que permite a criação de Pedidos de venda.

## Funcionalidades
- API que gerencia pedidos de venda
- A API permite operações CRUD para os pedidos de venda.

## Tecnologias Utilizadas
### Backend:
- **.NET 8.0**: A free, cross-platform, open source developer platform for building many different types of applications.
  - [GitHub](https://github.com/dotnet/core)
- **C#**: A modern object-oriented programming language developed by Microsoft.
  - [GitHub](https://github.com/dotnet/csharplang)

### Testing:
- **xUnit**: A free, open source, community-focused unit testing tool for the .NET Framework.
  - [GitHub](https://github.com/xunit/xunit)

### Databases:
- **PostgreSQL**: A powerful, open source object-relational database system.
  - [GitHub](https://github.com/postgres/postgres)
- **MongoDB**: A general purpose, document-based, distributed database.
  - [GitHub](https://github.com/mongodb/mongo)

## Configurando o Projeto

Clone o repositório:

```bash
git clone https://github.com/ferraro-net/Mouts.git
```

Certifique-se de ter o Docker e o Docker Compose instalados em seu ambiente de desenvolvimento.

### Executando o Docker Compose

No diretório raiz do projeto (onde está o arquivo `docker-compose.yml`), execute o comando:

```bash
docker-compose up -d
```

Isso irá construir as imagens Docker e iniciar os containers especificados no arquivo `docker-compose.yml`.

## Acessando a API

Após iniciar os containers, você pode acessar a API através do Swagger em:

[http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html)

Utilize o Postman ou outra ferramenta para fazer requisições HTTP à API para gerenciar as suas vendas.

## Contribuindo
Se você deseja contribuir com melhorias ou correções para o projeto Giga House, sinta-se à vontade para abrir uma issue ou enviar um pull request.

Agradecemos sua colaboração! 🙌

Este projeto foi desenvolvido com o intuito de demonstrar boas práticas de desenvolvimento e facilitar o gerenciamento de projetos através de uma API simples e eficiente.