﻿
using IOTHistoricalDataService.COR;
using IOTHistoricalDataService.COR.IRepository;
using IOTHistoricalDataService.COR.Repository;
using IOTHistoricalDataService.Entity;
using Microsoft.Extensions.Logging;

namespace IOTHistoricalDataService.Data
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly appDbContext _dbContext;
        private ILogger<IUnitOfWork> logger;

        // Define los campos privados para los repositorios
        private IDeviceRepository deviceRepository;
        private IDeviceRecordsRepository deviceRecordRepository;

        public UnitOfWork(
            appDbContext dbContext,
            ILogger<IUnitOfWork> logger,
            ILogger<GenericRepository<Device>> deviceLogger,
            ILogger<GenericRepository<DeviceRecord>> deviceRecordLogger)
      
        {
            _dbContext = dbContext;
            this.logger = logger;

            // Inicializa los repositorios en el constructor
            deviceRepository = new DeviceRepository(dbContext, deviceLogger);
            deviceRecordRepository = new DeviceRecordRepository(dbContext, deviceRecordLogger);
        }

        // Proporciona los repositorios a través de propiedades de solo lectura
        public async Task CompleteAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
        public IDeviceRecordsRepository DeviceRecordsRepository => deviceRecordRepository;

        public IDeviceRepository DeviceRepository => deviceRepository;


        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
