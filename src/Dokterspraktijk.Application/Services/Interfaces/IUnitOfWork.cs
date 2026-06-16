using Dokterspraktijk.Application.Repositories;

namespace Dokterspraktijk.Application.Services.Interfaces
{
    public interface IUnitOfWork
    {
        IDokterRepository Dokters { get; }
        IPatientRepository Patienten { get; }
        ITijdslotRepository Tijdsloten { get; }
        IAfspraakRepository Afspraken { get; }
        IDoktersattestRepository Doktersattesten { get; }
        IAfspraakCategorieRepository AfspraakCategorieen { get; }
        void SaveChanges();
    }
}
