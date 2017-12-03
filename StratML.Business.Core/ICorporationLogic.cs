using System;
using System.Collections.Generic;
using System.Text;
using StratML.Core;
using System.Threading;
using System.Threading.Tasks;

namespace StratML.Business.Core
{
    public interface ICorporationLogic
    {
        Task<ICollection<NameDescriptionType>> GetCorporations(CancellationToken token = default(CancellationToken));
        Task<Corporation> GetCorporation(string ids, CancellationToken token = default(CancellationToken));
        Task<Corporation> GetCorporation(string[] ids, CancellationToken token = default(CancellationToken));
        Task SaveCorporation(Corporation corporation, CancellationToken token = default(CancellationToken));
    }
}
