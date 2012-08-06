using NSoft.NFramework.Data.NHibernateEx.Domain;

namespace NSoft.NFramework.Web.TelerikEx
{
	public interface ITreeNodeEntityRadTreeViewAdapterAdapter<T> : IRadTreeViewAdapter<T> where T : class, ITreeNodeEntity<T> {}
}