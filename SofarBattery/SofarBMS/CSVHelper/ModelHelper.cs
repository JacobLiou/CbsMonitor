using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SofarBMS
{
    public abstract class ModelHelper
    {
        /// <summary>
        /// 当数据表列名和属性名不一致时可使用TableColumnNameAttribute为属性指定数据表列名
        /// 
        /// NotToModelAttribute 支持忽略不需要的属性值 即：忽略实体额外添加的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<T> ToModel<T>(DataTable table) where T : ICloneable, new()
        {
            List<T> list = new List<T>();

            if (table == null || table.Rows.Count == 0)
            {
                return list;
            }

            T obj = default(T);

            IEnumerable<PropertyInfo> propArr = typeof(T).GetProperties();

            foreach (DataRow item in table.Rows)
            {
                if (obj == null)
                {
                    obj = Activator.CreateInstance<T>();
                }
                else
                {
                    obj = (T) obj.Clone();
                }

                foreach (PropertyInfo prop in propArr)
                {
                    string realColumnName = prop.GetRealColumn(table);

                    // 属性名或特性标记字符串和数据表列名相同才填充值
                    if (table.Columns.Contains(realColumnName))
                    {
                        #region 填充

                        object val = item[realColumnName];

                        if (val == DBNull.Value)
                        {
                            if (
                                prop.PropertyType == typeof(long)
                                || prop.PropertyType == typeof(int)
                                || prop.PropertyType == typeof(float)
                                || prop.PropertyType == typeof(double)
                                || prop.PropertyType == typeof(decimal)
                            )
                            {
                                prop.SetValue(
                                    obj,
                                    -1
                                );
                            }

                            if (
                                prop.PropertyType == typeof(ulong)
                                || prop.PropertyType == typeof(uint)
                                || prop.PropertyType == typeof(ushort)
                            )
                            {
                                prop.SetValue(
                                    obj,
                                    0
                                );
                            }

                            if (prop.PropertyType == typeof(bool))
                            {
                                prop.SetValue(
                                    obj,
                                    false
                                );
                            }

                            if (prop.PropertyType == typeof(string))
                            {
                                prop.SetValue(
                                    obj,
                                    null
                                );
                            }

                            if (prop.PropertyType == typeof(DateTime))
                            {
                                prop.SetValue(
                                    obj,
                                    DateTime.MinValue
                                );
                            }
                        }
                        else
                        {
                            prop.SetValue(
                                obj,
                                val
                            );
                        }

                        #endregion
                    }
                }

                list.Add(obj);
            }

            return list;
        }

        /// <summary>
        /// NotToTableAttribute 支持忽略将不需要的属性值放入table
        /// 根据ColumnHeaderAttribute设置标题
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="modelList"></param>
        /// <returns></returns>
        public static DataTable ToTable<T>(IEnumerable<T> modelList)
        {
            if (modelList == null || !modelList.Any())
            {
                throw new NullReferenceException("The model is empty!");
            }

            DataTable table = new DataTable();

            table.TableName = typeof(T).GetTableName<T>();

            // 没标记NotToTableColumn的才填入table
            PropertyInfo[] propArr = typeof(T).GetProperties()
                .Where<PropertyInfo>(p => !p.IsDefined(typeof(NotToTableColumnAttribute))).ToArray();

            foreach (var item in propArr)
            {
                table.Columns.Add(item.GetHeader());
            }

            foreach (T item in modelList)
            {
                DataRow row = table.NewRow();

                for (int i = 0; i < propArr.Length; i++)
                {
                    row[propArr[i].GetHeader()] = propArr[i].GetValue(item);
                }

                table.Rows.Add(row);
            }

            return table;
        }

        /// <summary>
        /// 批量创建数据库实体
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableName"></param>
        /// <param name="tableComment"></param>
        /// <param name="toDirectory">DeskTop + "/TableModels"</param>
        /// <param name="nameSpace">DataModel</param>
        /// <param name="inheritBaseModel"></param>
        public static void BuildTheModelClassFromTable(DataTable table, string tableName, string tableComment = null,
            string toDirectory = null, string nameSpace = null, bool inheritBaseModel = true)
        {
            string theModelNameSpace = "DataModel";
            if (!string.IsNullOrEmpty(nameSpace))
            {
                theModelNameSpace = nameSpace;
            }

            if (string.IsNullOrEmpty(toDirectory))
            {
                toDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/TableModels";
            }

            TableModel modelClass = new TableModel(theModelNameSpace, inheritBaseModel);

            modelClass.ClassName = tableName;
            modelClass.TableComment = tableComment;
            modelClass.TableName = tableName;


            modelClass.PropertyCollection = table.Columns.Cast<DataColumn>().Select<DataColumn, ColumnProperty>(
                column => new ColumnProperty()
                {
                    ColumnType = column.DataType.ToString(),
                    ColumnName = column.ColumnName
                });

            if (!Directory.Exists(toDirectory))
            {
                Directory.CreateDirectory(toDirectory);
            }

            string filePath = toDirectory + "/" + modelClass.ClassName + ".cs";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            File.AppendAllText(filePath, modelClass.ToString());
        }

        private class TableModel
        {
            public TableModel(string nameSpace, bool inheritBaseModel = true)
            {
                this.NameSpace = nameSpace;
                this.InheritBaseModel = inheritBaseModel;
            }


            public string NameSpace { get; set; }
            public bool InheritBaseModel { get; set; }
            public string ClassName { get; set; }
            public string TableComment { get; set; }
            public string TableName { get; set; }


            public IEnumerable<ColumnProperty> PropertyCollection { get; set; }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                string inCodeBlock = "\n{\n";

                // 自动添加命名空间
                builder.Append("using System.ComponentModel.DataAnnotations.Schema;\nusing Helper.AbstractModel;\n\n");

                builder.Append("namespace " + NameSpace + inCodeBlock);

                if (!string.IsNullOrEmpty(this.TableComment))
                {
                    builder.Append("/// <summary>\n");
                    builder.Append("/// " + this.TableComment.Replace("\r\n", "\r\n/// "));
                    builder.AppendLine("\n/// </summary>");
                }

                if (this.TableName != null && this.TableName.Trim().Length > 0)
                {
                    builder.AppendLine("[Table(\"" + this.TableName + "\")]");
                }

                builder.Append("public class " + this.ClassName +
                               (this.InheritBaseModel ? " : BaseModel" : string.Empty) + inCodeBlock);

                foreach (var item in PropertyCollection)
                {
                    if (this.ClassName.Equals(item.ConvertColumnToProperty))
                    {
                        item.ConvertColumnToProperty += "Property";
                    }

                    builder.Append(item + "\n");
                }

                builder.AppendLine("public virtual string GetSelectCmdText()\n{"
                                   + "return \"SELECT * FROM " + this.TableName + ";\";\n}"
                );

                builder.AppendLine("public override string ToString()\n{"
                                   + $"return $\"{this.ClassName}: "
                                   + "["
                                   + string.Join(", ",
                                       this.PropertyCollection.Select<ColumnProperty, string>(
                                           column =>
                                               column.ConvertColumnToProperty + "={this." +
                                               column.ConvertColumnToProperty + "}"
                                       )
                                   )
                                   + "]\";\n}"
                );

                builder.Append("}\n}");

                return builder.ToString();
            }
        }

        private class ColumnProperty
        {
            public string ColumnType { get; set; }
            public string ColumnName { get; set; }

            public string ConvertColumnToProperty
            {
                get => !string.IsNullOrEmpty(ColumnName) ? ColumnName : ColumnName;

                set { this.ColumnName = value; }
            }

            public override string ToString()
            {
                return $"[Column(\"{this.ColumnName}\")]" + Environment.NewLine
                                                          + "public " + ColumnType + " " + ConvertColumnToProperty +
                                                          "{ get; set; }";
            }
        }
    }
}