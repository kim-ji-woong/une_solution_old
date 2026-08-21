using System.Collections;

namespace UnE.CCTV
{
	public interface IChangedDataManager
	{
		void SomethingChanged(ChangedData data);

		void RemoveData(ChangedData data);

		ArrayList GetDataList();
	}
}