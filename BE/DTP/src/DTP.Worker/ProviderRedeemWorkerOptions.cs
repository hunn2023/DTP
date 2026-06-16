using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Worker
{
    public class ProviderRedeemWorkerOptions
    {
        public bool Enabled { get; set; } = true;

        public int InitialDelaySeconds { get; set; } = 10;

        public int PollIntervalSeconds { get; set; } = 30;

        public int PollBatchSize { get; set; } = 20;

        public int EmailBatchSize { get; set; } = 20;
    }
}
