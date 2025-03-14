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
- **Mediator**: A behavioral design pattern that helps reduce chaotic dependencies between objects. It allows loose coupling by encapsulating object interaction.
  - [GitHub](https://github.com/jbogard/MediatR)
- **Automapper**: A convention-based object-object mapper that simplifies the process of mapping one object to another.
  - [GitHub](https://github.com/AutoMapper/AutoMapper)
- **Rebus**: A lean service bus implementation for .NET, providing a simple and flexible way to do messaging and queueing in .NET applications.
  - [GitHub](https://github.com/rebus-org/Rebus)

### Testing:
- **Faker**: A library for generating fake data for testing purposes, allowing for more realistic and diverse test scenarios.
  - [GitHub](https://github.com/bchavez/Bogus)
- **NSubstitute**: A friendly substitute for .NET mocking libraries, used for creating test doubles in unit testing.
  - [GitHub](https://github.com/nsubstitute/NSubstitute)

### Databases:
- **PostgreSQL**: A powerful, open source object-relational database system.
  - [GitHub](https://github.com/postgres/postgres)
- **MongoDB**: A general purpose, document-based, distributed database.
  - [GitHub](https://github.com/mongodb/mongo)
- **EF Core**: Entity Framework Core, a lightweight, extensible, and cross-platform version of Entity Framework, used for data access and object-relational mapping.
  - [GitHub](https://github.com/dotnet/efcore)

### Mensageria:
- **RabbitMQ**: Um broker de mensagens amplamente utilizado para comunicação assíncrona entre serviços.
  - [GitHub](https://github.com/rabbitmq)

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

