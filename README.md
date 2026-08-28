# SIGTI

Sistema de Gerenciamento de Chamados de Tecnologia da Informação.

O SIGTI é uma API para gerenciamento de chamados de suporte de TI, desenvolvida como projeto de estudo e portfólio com foco em arquitetura de software, Domain-Driven Design (DDD), Clean Architecture e CQRS.

> 🚧 **Projeto em desenvolvimento.**

---

## Sobre o projeto

O SIGTI tem como objetivo centralizar o gerenciamento de chamados de suporte técnico, permitindo organizar o atendimento através de:

- Tickets de suporte;
- Filas de atendimento;
- Departamentos;
- Técnicos;
- Atribuição automática de chamados;
- Controle de prioridade e categoria;
- Histórico de atribuições;
- Comentários;
- Paginação, filtros e ordenação.

A aplicação está sendo construída de forma incremental, buscando manter as regras de negócio concentradas no domínio e separar claramente as responsabilidades de cada camada.

---

## Arquitetura

O projeto utiliza uma arquitetura em camadas baseada em Clean Architecture e princípios de DDD.

```text
SIGTI
├── src
│   ├── SIGTI.API
│   ├── SIGTI.Application
│   ├── SIGTI.Domain
│   └── SIGTI.Infrastructure
│
└── tests
    ├── SIGTI.Application.Tests
    └── SIGTI.Domain.Tests
```

### SIGTI.Domain

Contém as regras e conceitos centrais do sistema.

Entre os principais componentes estão:

- Entities;
- Value Objects;
- Enums;
- Domain Exceptions;
- Factories;
- Regras de negócio.

Principais entidades:

- `Ticket`
- `TicketAssignment`
- `SupportQueue`
- `SupportQueueMember`
- `User`
- `Department`
- `Comment`

---

### SIGTI.Application

Contém os casos de uso da aplicação.

A comunicação é organizada utilizando Commands e Queries através do MediatR.

Exemplos:

```text
Commands
└── CreateTicket

Queries
├── GetTicketById
└── ListTickets
```

A camada também contém:

- Validators;
- Pipeline Behaviors;
- DTOs/Responses;
- Interfaces de persistência;
- Interfaces de serviços;
- Modelos compartilhados, como paginação.

---

### SIGTI.Infrastructure

Responsável pelas implementações externas da aplicação.

Inclui:

- Entity Framework Core;
- PostgreSQL;
- Repositories;
- Unit of Work;
- Configurações de persistência;
- Database Seeder;
- Geração sequencial dos números dos tickets;
- Serviços de infraestrutura.

A numeração dos tickets utiliza uma PostgreSQL Sequence para gerar números sequenciais.

---

### SIGTI.API

É a camada de entrada da aplicação.

Responsável por:

- Controllers;
- Configuração da aplicação;
- Dependency Injection;
- Exception Handling;
- Swagger/OpenAPI;
- Exposição dos endpoints HTTP.

---

## Principais conceitos utilizados

### Domain-Driven Design

As principais regras de negócio permanecem no domínio.

Exemplo:

```csharp
queue.AddMember(technician, maxConcurrentTickets);
```

A própria entidade `SupportQueue` é responsável por controlar a inclusão de membros na fila.

---

### CQRS

Commands representam operações que alteram o estado da aplicação.

Queries representam operações de leitura.

Exemplo:

```text
CreateTicketCommand
        ↓
     escrita

GetTicketByIdQuery
        ↓
      leitura
```

---

### MediatR

Utilizado para desacoplar Controllers dos casos de uso e implementar o fluxo de Commands, Queries e Pipeline Behaviors.

---

### FluentValidation

Utilizado para validar dados de entrada antes da execução dos casos de uso.

A validação é integrada ao pipeline do MediatR através de um `ValidationBehavior`.

---

### Repository + Unit of Work

A persistência é abstraída através de interfaces na Application e implementações na Infrastructure.

---

### Exception Handling

Exceções da aplicação são tratadas centralizadamente através de um Global Exception Handler, retornando respostas HTTP padronizadas.

---

## Funcionalidades implementadas

### Tickets

- [x] Criar ticket;
- [x] Geração automática de número;
- [x] Atribuição automática de técnico;
- [x] Buscar ticket por ID;
- [x] Listar tickets;
- [x] Paginação;
- [x] Filtros;
- [x] Ordenação;
- [x] Controle de prioridade;
- [x] Controle de categoria;
- [x] Histórico de atribuições;
- [x] Comentários no domínio.

### Infraestrutura

- [x] Entity Framework Core;
- [x] PostgreSQL;
- [x] Migrations;
- [x] Database Seeder;
- [x] Dependency Injection;
- [x] Global Exception Handler;
- [x] Swagger/OpenAPI;
- [x] PostgreSQL Sequence para numeração dos tickets.

### Testes

- [x] Testes de domínio;
- [x] Testes de Commands;
- [x] Testes de Queries;
- [x] Testes de Validators;
- [x] Testes com Moq para isolamento de dependências na Application.

---

## Paginação, filtros e ordenação

A listagem de tickets suporta paginação através de `PagedResult<T>`.

Exemplo:

```http
GET /api/tickets?page=1&pageSize=20
```

Os resultados podem ser filtrados por critérios como:

- Status;
- Prioridade;
- Categoria;
- Departamento;
- Fila;
- Técnico.

Também é possível controlar a ordenação por campos definidos pela aplicação.

Exemplo conceitual:

```http
GET /api/tickets?page=1&pageSize=20&sortBy=Priority&sortDirection=Ascending
```

---

## Testes

Os testes estão separados por responsabilidade:

```text
tests
├── SIGTI.Domain.Tests
└── SIGTI.Application.Tests
```

Os testes do domínio validam principalmente regras de negócio.

Os testes da Application validam o comportamento dos casos de uso e a interação com suas dependências.

Para executar todos os testes:

```bash
dotnet test
```

---

## Tecnologias

- C#
- .NET
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- MediatR
- FluentValidation
- xUnit
- FluentAssertions
- Moq
- Swagger / OpenAPI

---

## Executando o projeto

### Pré-requisitos

- .NET SDK;
- PostgreSQL;
- Banco de dados configurado para a aplicação.

### Configuração

Configure a connection string no ambiente de desenvolvimento da API.

Exemplo:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=SIGTI;Username=postgres;Password=sua-senha"
  }
}
```

> Nunca versione credenciais reais no repositório.

### Executando as migrations

```bash
dotnet ef database update
```

### Executando a API

```bash
dotnet run --project src/SIGTI.API
```

Em ambiente de desenvolvimento, a API disponibiliza a documentação através de Swagger/OpenAPI.

---

## Banco de dados e Seed

Na inicialização da aplicação, o `DatabaseSeeder` prepara os dados básicos necessários ao ambiente de desenvolvimento.

Atualmente o cenário inicial contempla:

- Departamento de Tecnologia da Informação;
- Fila de Suporte Técnico;
- Usuário do sistema;
- Usuário técnico;
- Associação do técnico à fila.

O objetivo do seed é deixar o ambiente pronto para testar o fluxo de criação e atribuição automática de chamados.

---

## Status do projeto

O projeto encontra-se em desenvolvimento.

O objetivo atual é continuar expandindo os casos de uso do sistema e fortalecer sua cobertura de testes, mantendo as regras de negócio encapsuladas no domínio e a separação de responsabilidades entre as camadas.

---

## Próximos passos

Algumas funcionalidades planejadas:

- [ ] Assumir atendimento;
- [ ] Iniciar atendimento;
- [ ] Transferir ticket;
- [ ] Resolver ticket;
- [ ] Fechar ticket;
- [ ] Reabrir ticket;
- [ ] Histórico completo do ticket;
- [ ] Autenticação e autorização;
- [ ] Gestão de usuários;
- [ ] Gestão de departamentos;
- [ ] Gestão de filas;
- [ ] Dashboard;
- [ ] SLA;
- [ ] Testes de integração.

---

## Objetivo

Além de construir um sistema funcional de gerenciamento de chamados, o SIGTI está sendo desenvolvido como um projeto de aprendizado contínuo em:

- Arquitetura de software;
- Domain-Driven Design;
- Clean Architecture;
- CQRS;
- Desenvolvimento de APIs;
- Persistência com Entity Framework Core;
- Testes automatizados;
- Boas práticas no ecossistema .NET.

O foco do projeto não está apenas em desenvolver funcionalidades, mas também em compreender e aplicar os princípios por trás das decisões arquiteturais.

---

## Licença

Este projeto ainda não possui uma licença definida.
