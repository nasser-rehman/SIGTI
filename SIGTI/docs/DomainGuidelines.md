# Domain Guidelines

## Entidades

- Toda entidade deve ser `sealed`.
- Toda entidade herda de `BaseEntity`.
- Nenhuma entidade acessa banco de dados.
- Nenhuma entidade conhece Repository.
- Nenhuma entidade conhece DbContext.

---

## Relacionamentos

As entidades sempre recebem outras entidades.

✔ Correto

user.ChangeDepartment(department);

ticket.AssignTechnician(user);

queue.AddMember(user);

❌ Evitar

user.ChangeDepartment(departmentId);

ticket.AssignTechnician(userId);

---

## Coleções

Sempre utilizar

private readonly List<T>

+

IReadOnlyCollection<T>

Nunca expor ICollection pública.

---

## Atualizações

Toda alteração passa por métodos.

UpdateName()

ChangeDepartment()

Deactivate()

Resolve()

Nunca utilizar setters públicos.

---

## Validações

Sempre validar antes de alterar estado.

Mensagens de erro padronizadas.

Sempre utilizar DomainException.

---

## Construtores

Construtores reutilizam métodos do domínio.

Nunca duplicar validações.

---

## Navegações

Sempre que fizer sentido, manter Navigation Property.

Não remover navegações apenas para "facilitar o EF".

---

## Guid

O Domain não trabalha com Guid quando existe uma entidade correspondente.

Quem conhece Guid é a Application.
