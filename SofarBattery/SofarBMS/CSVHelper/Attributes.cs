using System;

namespace SofarBMS
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }
    }
    
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }
    }
    
    [AttributeUsage(AttributeTargets.Property)]
    public class NotToModelPropertyAttribute : Attribute
    {
    }
    
    /// <summary>
    /// ListView/DataGrid修改标题名
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnHeaderAttribute : Attribute
    {
        public string HeaderName { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class NotToTableColumnAttribute : Attribute
    {
    }

    /// <summary>
    /// 标记此特性的属性在使用ViewExtendMethod.AddWithoutTemplateByNotShowInView时不会显示在datagrid中
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NotShowInViewAttribute : Attribute
    {
    }

    /// <summary>
    /// 标记此特性的属性在使用ViewExtendMethod.AddWithoutTemplateByNotShowInView时不会显示在datagrid中
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NotShowInDataGridAttribute : Attribute
    {
    }

    /// <summary>
    /// 标记此特性的属性ViewExtendMethod.AddWithoutTemplateByNotShowInView时可以被编辑
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CanEditAttribute : Attribute
    {
    }
}