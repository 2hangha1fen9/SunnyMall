//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrdersDetails
    {
        public int DetailID { get; set; }
        public int OrdersID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public int States { get; set; }
    
        public virtual Products Products { get; set; }
        public virtual Orders Orders { get; set; }
    }
}
