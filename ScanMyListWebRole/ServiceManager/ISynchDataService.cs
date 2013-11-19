namespace SynchWebRole.ServiceManager
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Web;


    [ServiceContract]
    public interface ISynchDataService : IAccountManager, IBusinessManager, IInventoryManager, IRecordManager,
                                        ICustomerManager, ISupplierManager
    {
        
    }
}
