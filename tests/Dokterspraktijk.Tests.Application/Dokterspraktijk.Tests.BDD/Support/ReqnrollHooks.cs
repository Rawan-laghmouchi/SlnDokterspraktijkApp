using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Application.Services.Implementation;
using Dokterspraktijk.Application.Services.Interfaces;
using Dokterspraktijk.Infrastructure.Fakes;
using Reqnroll.BoDi;

namespace Dokterspraktijk.Tests.BDD.Support
{
    [Binding]
    public class ReqnrollHooks
    {
        private readonly IObjectContainer _container;

        public ReqnrollHooks(IObjectContainer container)
        {
            _container = container;
        }

        [BeforeScenario]
        public void RegisterDependencies()
        {
            DokterspraktijkScenarioContext scenarioContext = new DokterspraktijkScenarioContext();

            FakeDokterspraktijkDatastore dataStore = new FakeDokterspraktijkDatastore();

            IDokterRepository dokterRepository = new FakeDokterRepository(dataStore);
            IPatientRepository patientRepository = new FakePatientRepository(dataStore);
            ITijdslotRepository tijdslotRepository = new FakeTijdslotRepository(dataStore);
            IAfspraakRepository afspraakRepository = new FakeAfspraakRepository(dataStore, tijdslotRepository);
            IDoktersattestRepository doktersattestRepository = new FakeDoktersattestRepository(dataStore);
            IAfspraakCategorieRepository afspraakCategorieRepository = new FakeAfspraakCategorieRepository(dataStore);

            IUnitOfWork unitOfWork = new FakeUnitOfWork(
                dokterRepository,
                patientRepository,
                tijdslotRepository,
                afspraakRepository,
                doktersattestRepository,
                afspraakCategorieRepository);

            IAfspraakService afspraakService = new AfspraakService(unitOfWork);
            ITijdslotService tijdslotService = new TijdslotService(unitOfWork);
            IPatientService patientService = new PatientService(unitOfWork);
            IDoktersattestService doktersattestService = new DoktersattestService(unitOfWork);

            _container.RegisterInstanceAs(scenarioContext);

            _container.RegisterInstanceAs(dokterRepository);
            _container.RegisterInstanceAs(patientRepository);
            _container.RegisterInstanceAs(tijdslotRepository);
            _container.RegisterInstanceAs(afspraakRepository);
            _container.RegisterInstanceAs(doktersattestRepository);
            _container.RegisterInstanceAs(afspraakCategorieRepository);
            _container.RegisterInstanceAs(unitOfWork);

            _container.RegisterInstanceAs(tijdslotService);
            _container.RegisterInstanceAs(afspraakService);
            _container.RegisterInstanceAs(patientService);
            _container.RegisterInstanceAs(doktersattestService);
        }
    }
}