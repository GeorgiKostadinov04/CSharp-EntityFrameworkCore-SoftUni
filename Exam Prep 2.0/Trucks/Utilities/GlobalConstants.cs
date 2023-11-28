using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trucks.Utilities
{
    public static class GlobalConstants
    {
        //Despatcher
        public const int despatherNameMaxLength = 40;
        public const int despatcherNameMinLength = 2;

        //Truck
        public const int truckRegNumberLength = 8;
        public const int truckVinNumberLength = 17;
        public const int truckTankCapacityMaxValue = 1420;
        public const int truckTankCapacityMinValue = 950;
        public const int truckCargoCapacityMaxValue = 29000;
        public const int truckCargoCapacityMinValue = 5000;

        //Client
        public const int clientNameMaxLength = 40;
        public const int clientNameMinLength = 3;
        public const int clientNationalityMaxLength = 40;
        public const int clientNationalityMinLength = 2;
    }
}
