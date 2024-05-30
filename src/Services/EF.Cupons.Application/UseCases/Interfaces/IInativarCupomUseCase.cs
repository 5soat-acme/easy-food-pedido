using EF.Core.Commons.Communication;

namespace EF.Cupons.Application.UseCases.Interfaces;

public interface IInativarCupomUseCase
{
    Task<OperationResult> Handle(Guid id);
}