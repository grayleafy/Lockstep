

using System;




public interface IModel
{
    Action<IModel> ModifiedEvent { get; set; }
}



