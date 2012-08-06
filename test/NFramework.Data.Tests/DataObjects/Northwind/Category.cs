using System;

namespace NSoft.NFramework.Data.DataObjects.Northwind {
    /// <summary>
    /// Save Entity by Procedure / Query 를 위해 사용
    /// </summary>
    [Serializable]
    internal class Category : DataObjectBase {
        public Category() {
            CategoryId = -1;
        }

        public Category(string categoryName)
            : this() {
            CategoryName = categoryName;
        }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string Description { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(CategoryId, CategoryName);
        }

        public override string ToString() {
            return string.Format("Category# Id=[{0}], Name=[{1}], Description=[{2}]", CategoryId, CategoryName, Description);
        }
    }
}