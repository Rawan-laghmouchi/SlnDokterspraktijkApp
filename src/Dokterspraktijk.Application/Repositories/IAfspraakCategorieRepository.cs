using Dokterspraktijk.Domain.Entities;

namespace Dokterspraktijk.Application.Repositories
{
    public interface IAfspraakCategorieRepository
    {
        List<AfspraakCategorie> GeefAlleCategorieen();
    }
}
