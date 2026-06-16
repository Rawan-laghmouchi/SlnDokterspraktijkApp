using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Application.Services.Interfaces;

namespace Dokterspraktijk.Infrastructure.Fakes
{
    public class FakeUnitOfWork : IUnitOfWork
    {
        private IDokterRepository _dokterRepository;
        private IPatientRepository _patientRepository;
        private ITijdslotRepository _tijdslotRepository;
        private IAfspraakRepository _afspraakRepository;
        private IDoktersattestRepository _doktersattestRepository;
        private IAfspraakCategorieRepository _afspraakCategorieRepository;

        public FakeUnitOfWork(
            IDokterRepository dokterRepository,
            IPatientRepository patientRepository,
            ITijdslotRepository tijdslotRepository,
            IAfspraakRepository afspraakRepository,
            IDoktersattestRepository doktersattestRepository,
            IAfspraakCategorieRepository afspraakCategorieRepository)
        {
            _dokterRepository = dokterRepository;
            _patientRepository = patientRepository;
            _tijdslotRepository = tijdslotRepository;
            _afspraakRepository = afspraakRepository;
            _doktersattestRepository = doktersattestRepository;
            _afspraakCategorieRepository = afspraakCategorieRepository;
        }

        public IDokterRepository Dokters => _dokterRepository;

        public IPatientRepository Patienten => _patientRepository;

        public ITijdslotRepository Tijdsloten => _tijdslotRepository;

        public IAfspraakRepository Afspraken => _afspraakRepository;

        public IDoktersattestRepository Doktersattesten => _doktersattestRepository;

        public IAfspraakCategorieRepository AfspraakCategorieen => _afspraakCategorieRepository;

        public void SaveChanges()
        {
        }
    }
}