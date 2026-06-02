using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Application.Services.Implementation;
using Dokterspraktijk.Application.Services.Interfaces;
using Dokterspraktijk.Infrastructure.Fakes;
using Reqnroll;
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

            ITijdslotService tijdslotService = new TijdslotService(
                dokterRepository,
                tijdslotRepository);

            IAfspraakService afspraakService = new AfspraakService(
                patientRepository,
                dokterRepository,
                tijdslotRepository,
                afspraakRepository);

            IPatientService patientService = new PatientService(
                patientRepository,
                dokterRepository);

            IDoktersattestService doktersattestService = new DoktersattestService(
                patientRepository,
                dokterRepository,
                afspraakRepository,
                doktersattestRepository);

            _container.RegisterInstanceAs(scenarioContext);

            _container.RegisterInstanceAs(dokterRepository);
            _container.RegisterInstanceAs(patientRepository);
            _container.RegisterInstanceAs(tijdslotRepository);
            _container.RegisterInstanceAs(afspraakRepository);
            _container.RegisterInstanceAs(doktersattestRepository);

            _container.RegisterInstanceAs(tijdslotService);
            _container.RegisterInstanceAs(afspraakService);
            _container.RegisterInstanceAs(patientService);
            _container.RegisterInstanceAs(doktersattestService);
        }
    }
}