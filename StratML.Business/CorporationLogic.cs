using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StratML.Core;
using StratML.Data.Core;
using StratML.Business.Core;
using StratML.Core.Custom;
using StratML.Core.Three;

namespace StratML.Business
{
    public class CorporationLogic : ICorporationLogic
    {
        protected ICorporationAdapater CorporationAdapater { get; private set; }
        public CorporationLogic(ICorporationAdapater corpData)
        {
             this.CorporationAdapater = corpData;
        }
        public Task<Corporation> GetCorporation(string ids, CancellationToken token = default(CancellationToken))
        {
            return GetCorporation(ids.Split(','), token);
        }

        public Task<Corporation> GetCorporation(string[] ids, CancellationToken token = default(CancellationToken))
        {
            return this.CorporationAdapater.GetCorporation(ids, token);
        }

        public Task<ICollection<NameDescriptionType>> GetCorporations(CancellationToken token = default(CancellationToken))
        {
            return this.CorporationAdapater.GetCorporations(token);
        }

        public Task SaveCorporation(Corporation corporation, CancellationToken token = default(CancellationToken))
        {
            //TODO: Validation and ID population
            if (corporation.NameDescription.Identifier == null || corporation.NameDescription.Identifier.Length == 0)
                corporation.NameDescription.Identifier = new string[] { Guid.NewGuid().ToString() };
            return this.CorporationAdapater.SaveCorporation(corporation, token);
        }
    }
}
