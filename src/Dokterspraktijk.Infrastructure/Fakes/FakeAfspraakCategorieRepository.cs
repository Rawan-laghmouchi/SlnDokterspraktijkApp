using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Domain.Entities;

namespace Dokterspraktijk.Infrastructure.Fakes
{
    public class FakeAfspraakCategorieRepository : IAfspraakCategorieRepository
    {
        private FakeDokterspraktijkDatastore _dataStore;

        public FakeAfspraakCategorieRepository(FakeDokterspraktijkDatastore dataStore)
        {
            _dataStore = dataStore;
        }

        public List<AfspraakCategorie> GeefAlleCategorieen()
        {
            return _dataStore.AfspraakCategorieen
                .OrderBy(categorie => categorie.Naam)
                .ToList();
        }
    }
}