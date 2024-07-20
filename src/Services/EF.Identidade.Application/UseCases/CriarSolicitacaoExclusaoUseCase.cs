using EF.Core.Commons.Communication;
using EF.Core.Commons.UseCases;
using EF.Core.Commons.ValueObjects;
using EF.Identidade.Application.DTOs.Requests;
using EF.Identidade.Application.UseCases.Interfaces;
using EF.Identidade.Domain.Models;
using EF.Identidade.Domain.Repository;

namespace EF.Identidade.Application.UseCases;

public class CriarSolicitacaoExclusaoUseCase : CommonUseCase, ICriarSolicitacaoExclusaoUseCase
{
    private readonly ISolicitacaoExclusaoRepository _solicitacaoExclusaoRepository;

    public CriarSolicitacaoExclusaoUseCase(ISolicitacaoExclusaoRepository solicitacaoExclusaoRepository)
    {
        _solicitacaoExclusaoRepository = solicitacaoExclusaoRepository;
    }

    public async Task<OperationResult<Guid>> Handle(CriarSolicitacaoExclusaoDto dto)
    {
        if (dto.ClienteId == Guid.Empty)
            return OperationResult<Guid>.Failure("Cliente inválido");


        if (!await ValidaSolicitacaoExistente(dto.ClienteId!.Value))
            return OperationResult<Guid>.Failure("Solicitação já efetuada!");


        var solicitacaoEsclusao = new SolicitacaoExclusao(dto.ClienteId!.Value, dto.Nome, new Endereco(dto.Rua, dto.Numero, dto.Complemento,
            dto.Cidade, dto.Cidade, dto.Estado, dto.Cep), dto.NumeroTelefone);
        _solicitacaoExclusaoRepository.Criar(solicitacaoEsclusao);
        await PersistData(_solicitacaoExclusaoRepository.UnitOfWork);
        return OperationResult<Guid>.Success(solicitacaoEsclusao.Id);
    }

    private async Task<bool> ValidaSolicitacaoExistente(Guid clienteId)
    {
        var retorno = await _solicitacaoExclusaoRepository.BuscarPorClienteId(clienteId);
        return retorno is null;
    }
}