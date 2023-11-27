using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boardgames.Utilities
{
    public static class Values
    {
        //Creator
        public const int creatorFirstNameMaxValue = 7;
        public const int creatorFirstNameMinValue = 2;

        public const int creatorLastNameMaxValue = 7;
        public const int creatorLastNameMinValue = 2;

        //Boardgame
        public const int boardgameNameMaxValue = 20;
        public const int boardgameNameMinValue = 10;

        public const int boardgameRangeMaxValue = 10;
        public const int boardgameRangeMinValue = 1;

        public const int boardgameYearMaxValue = 2023;
        public const int boardgameYearMinValue = 2018;

        //Seller
        public const int sellerNameMaxValue = 20;
        public const int sellerNameMinValue = 5;

        public const int sellerAddressMaxValue = 30;
        public const int sellerAddressMinValue = 2;
    }
}
