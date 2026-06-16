using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Application.Services.Implementation;
using Dokterspraktijk.Application.Services.Interfaces;
using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.Domain.Enums;
using Dokterspraktijk.Infrastructure.Fakes;

namespace Dokterspraktijk.Tests.xUnit.Support
{
    public class DokterspraktijkServiceTestContext
    {
        private int _volgendTijdslotId;

        public FakeDokterspraktijkDatastore DataStore { get; }

        public IDokterRepository DokterRepository { get; }
        public IPatientRepository PatientRepository { get; }
        public ITijdslotRepository TijdslotRepository { get; }
        public IAfspraakRepository AfspraakRepository { get; }
        public IDoktersattestRepository DoktersattestRepository { get; }
        public IAfspraakCategorieRepository AfspraakCategorieRepository { get; }
        public IUnitOfWork UnitOfWork { get; }

        public ITijdslotService TijdslotService { get; }
        public IAfspraakService AfspraakService { get; }
        public IPatientService PatientService { get; }
        public IDoktersattestService DoktersattestService { get; }

        public DokterspraktijkServiceTestContext()
        {
            _volgendTijdslotId = 1000;

            DataStore = new FakeDokterspraktijkDatastore();

            DokterRepository = new FakeDokterRepository(DataStore);
            PatientRepository = new FakePatientRepository(DataStore);
            TijdslotRepository = new FakeTijdslotRepository(DataStore);
            AfspraakRepository = new FakeAfspraakRepository(DataStore, TijdslotRepository);
            DoktersattestRepository = new FakeDoktersattestRepository(DataStore);
            AfspraakCategorieRepository = new FakeAfspraakCategorieRepository(DataStore);

            UnitOfWork = new FakeUnitOfWork(
                DokterRepository,
                PatientRepository,
                TijdslotRepository,
                AfspraakRepository,
                DoktersattestRepository,
                AfspraakCategorieRepository);

            TijdslotService = new TijdslotService(UnitOfWork);

            AfspraakService = new AfspraakService(UnitOfWork);

            PatientService = new PatientService(UnitOfWork);

            DoktersattestService = new DoktersattestService(UnitOfWork);
        }

        public Tijdslot VoegTijdslotToe(
            string dokterNaam,
            DateOnly datum,
            TimeOnly tijd,
            TijdslotStatus status)
        {
            Dokter? dokter = DokterRepository.ZoekOpNaam(dokterNaam);

            if (dokter == null)
            {
                throw new InvalidOperationException($"Dokter {dokterNaam} bestaat niet.");
            }

            Tijdslot tijdslot = new Tijdslot(
                _volgendTijdslotId,
                dokter.Id,
                datum,
                tijd,
                status);

            _volgendTijdslotId++;

            TijdslotRepository.VoegToe(tijdslot);

            return tijdslot;
        }

        public Afspraak? ZoekAfspraak(
            string patientVoornaam,
            string patientAchternaam,
            string dokterNaam,
            DateOnly datum,
            TimeOnly tijd)
        {
            Patient? patient = PatientRepository.ZoekOpNaam(patientVoornaam, patientAchternaam);
            Dokter? dokter = DokterRepository.ZoekOpNaam(dokterNaam);

            if (patient == null || dokter == null)
            {
                return null;
            }

            return AfspraakRepository.ZoekOpPatientDokterDatumEnTijd(
                patient.Id,
                dokter.Id,
                datum,
                tijd);
        }

        public Tijdslot? ZoekTijdslot(
            string dokterNaam,
            DateOnly datum,
            TimeOnly tijd)
        {
            Dokter? dokter = DokterRepository.ZoekOpNaam(dokterNaam);

            if (dokter == null)
            {
                return null;
            }

            return TijdslotRepository.ZoekVoorDokterOpDatumEnTijd(
                dokter.Id,
                datum,
                tijd);
        }
    }
}