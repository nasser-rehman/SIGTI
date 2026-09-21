using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Features.SupportQueues.Commands.UpdateSupportQueue;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.Tests.Builders;
using Xunit;

namespace SIGTI.Application.Tests.Features.SupportQueues.Commands.UpdateSupportQueue
{
    public class UpdateSupportQueueCommandHandlerTests
    {
        private readonly Mock<ISupportQueueRepository> _supportQueueRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateSupportQueueCommandHandler _handler;

        public UpdateSupportQueueCommandHandlerTests()
        {
            _supportQueueRepositoryMock = new Mock<ISupportQueueRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new UpdateSupportQueueCommandHandler(
                _supportQueueRepositoryMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Handle_WhenQueueExistsAndDataIsValid_ShouldUpdateAndReturnResponse()
        {
            // Arrange
            var queue = new SupportQueueBuilder().Build();
            var command = new UpdateSupportQueueCommand(
                queue.Id,
                "Fila N2 Suporte Avançado",
                "Atendimento N2 técnico."
            );

            _supportQueueRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(queue.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(queue);

            _supportQueueRepositoryMock
                .Setup(r =>
                    r.ExistsByNameAsync(
                        "Fila N2 Suporte Avançado",
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(queue.Id);
            result.Name.Should().Be("Fila N2 Suporte Avançado");
            result.Description.Should().Be("Atendimento N2 técnico.");
            result.UpdatedAt.Should().NotBeNull();

            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WhenQueueDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var command = new UpdateSupportQueueCommand(
                nonExistentId,
                "Nome",
                "Descrição"
            );

            _supportQueueRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync((SupportQueue?)null);

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WhenNameAlreadyExists_ShouldThrowDomainException()
        {
            // Arrange
            var queue = new SupportQueueBuilder().Build();
            var command = new UpdateSupportQueueCommand(
                queue.Id,
                "Nome Existente",
                "Descrição"
            );

            _supportQueueRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(queue.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(queue);

            _supportQueueRepositoryMock
                .Setup(r =>
                    r.ExistsByNameAsync(
                        "Nome Existente",
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(true);

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<DomainException>()
                .WithMessage("Fila de suporte com o mesmo nome já existente.");
        }

        [Fact]
        public async Task Handle_WhenNameUnchanged_ShouldNotCheckExistsByName()
        {
            // Arrange
            var queue = new SupportQueueBuilder().Build();
            var command = new UpdateSupportQueueCommand(
                queue.Id,
                queue.Name,
                "Nova Descrição"
            );

            _supportQueueRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(queue.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(queue);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Description.Should().Be("Nova Descrição");
            _supportQueueRepositoryMock.Verify(
                r =>
                    r.ExistsByNameAsync(
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()
                    ),
                Times.Never
            );
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }
    }
}
