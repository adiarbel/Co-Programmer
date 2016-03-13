﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace CoProService
{
    [ServiceContract(CallbackContract = typeof(ICoProServiceCallback))]
    interface ICoProService
    {
        [OperationContract]
        void IntializePosition(string file, int position);
        
    }

    interface ICoProServiceCallback
    {
        [OperationContract]
        void NewEditorAdded(string file,int position);
    }
}
