using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Horizon.Sample.Grpccontract
{
    [DataContract]
    public class Student
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }
        [DataMember(Order = 2)]
        public int Age { get; set; }
        [DataMember(Order = 3)]
        public int No { get; set; }
    }


    [DataContract]
    public class ResponeConext
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }
    }

    [ServiceContract]
    public interface IStudentCollection
    {
        [OperationContract]
        ValueTask<Student> GetStudentAsync(ResponeConext conext);
    }


    [ServiceContract]
    public interface IChat
    {
        [OperationContract]
        IAsyncEnumerable<Student> JoinAndChat(IAsyncEnumerable<Student> message);
    }
}
