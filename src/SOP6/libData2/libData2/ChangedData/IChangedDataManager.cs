using System.Collections;

namespace SDMS
{
	public interface IChangedDataManager
	{
		void SomethingChanged(ChangedData data);

		void RemoveData(ChangedData data);

		ArrayList GetDataList();
	}
}