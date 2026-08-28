ADR-001

ApplicationDbContext implementa IUnitOfWork.

Motivo:

DbContext já implementa naturalmente o padrão Unit Of Work.

Evita uma classe desnecessária.

Data:

28/07/2026


ADR-002

Entidades recebem entidades ao invés de Guid.

Motivo:

Maior expressividade do domínio.

Maior encapsulamento.

Menor acoplamento com persistência.

Data:

31/07/2026
