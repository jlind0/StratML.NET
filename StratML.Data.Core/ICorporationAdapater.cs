using System;
using System.Collections.Generic;
using System.Text;
using StratML.Core;
using System.Threading;
using System.Threading.Tasks;

namespace StratML.Data.Core
{
    public interface ICorporationAdapater
    {
        Task<ICollection<NameDescriptionType>> GetCorporations(CancellationToken token = default(CancellationToken));
        Task<Corporation> GetCorporation(string[] id, CancellationToken token = default(CancellationToken));
        Task SaveCorporation(Corporation corporation, CancellationToken token = default(CancellationToken));
    }
}
