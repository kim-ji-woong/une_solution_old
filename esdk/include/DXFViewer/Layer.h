#pragma once

namespace DXFViewer
{
	interface class IPainter;
	ref class Shape;
	ref class LineType;
	ref class Block;
	ref class ShapeGroupOption;

	public ref class Layer
	{
	public:
		Layer(IPainter^ owner);
		Layer(IPainter^ owner, LineType^ lineType);
		virtual ~Layer(void);

	public:
		void Add(Shape^ obj);
		// pObject가 Layer에 존재하면 pObject를 삭제하고 true를 리턴한다.
		bool Remove(Shape^ obj);
		// 모든 Object를 삭제하면 true,
		// 삭제하지 못한 Object가 존재하면 false를 리턴한다.
		bool RemoveAll();
		// pObject가 Layer에 존재하면 true를 리턴한다.
		bool Find(Shape^ obj);
		void Reset();
		void Init();
		virtual bool Draw(System::Drawing::Graphics^ g, bool bDrawText);
		virtual bool DrawObject(Shape^ obj, System::Drawing::Graphics^ g, bool bDrawText);

		//void SetLineColor(System::Drawing::Color^ col);
		void SetLineType(LineType^ type);
		//void SetFrozen(bool isFrozen);
		//void SetLock(bool isLock);
		//void SetLayerName(System::String^ strLayerName);

		//System::Drawing::Color^ GetLineColor();
		LineType^ GetLineType();
		//bool IsLock();
		
		//System::String^ GetLayerName();

		//void SetOwner(IPainter^ owner);
		//IPainter^ GetOwner();
		// 모든 객체들을 현재의 위치로부터 (x, y) 만큼 이동시킨다.
		void MoveAll(double x, double y);

		void CalcGroup(int nGroupItemMinCount, int nGroupItemDistance);
		void CalcGroup(int nGroupItemMinCount, int nGroupItemDistance, DXFViewer::ShapeGroupOption^ option);

	//protected:
		bool DrawHatch(System::Drawing::Graphics^ g, bool bDrawText);
		bool DrawExceptHatch(System::Drawing::Graphics^ g, bool bDrawText);
		bool DrawGroup(System::Drawing::Graphics^ g, bool bDrawText);

		Shape^ SelectObject(double x, double y);

	private:
		System::Collections::ArrayList^ GetGroupItems(int nGroupItemMinCount, int nGroupItemDistance, Shape^ obj, System::Collections::ArrayList^ arrShapes, System::Drawing::Point% ptOrigin);
		void GetGroupItems(int nGroupItemMinCount, int nGroupItemDistance, Shape^ obj, System::Collections::ArrayList^ arrShapes, System::Collections::ArrayList^ arrGroupItems, System::Drawing::Point% ptOrigin);
		void ClearShapeGroup();
		void AddShapeGroup(System::Collections::ArrayList^ arrGroupItems, DXFViewer::ShapeGroupOption^ option);
		int GetDistance(System::Drawing::Point% pt1, System::Drawing::Point% pt2);

		void DrawObjectGL(Shape^ obj);
		
	public:
		property System::Collections::ArrayList^ Shapes
		{
			System::Collections::ArrayList^ get() { return m_listObject; }
		}

		property bool Hidden
		{
			bool get() { return m_isHidden; }
			void set(bool value) { m_isHidden = value; }
		}

		property bool Lock
		{
			bool get() { return m_isLock; }
			void set(bool value) { m_isLock = value; }
		}

		property bool Frozen
		{
			bool get() { return m_isFrozen; }
			void set(bool value) { m_isFrozen = value; }
		}

		/*property LineType^ LineType
		{
			LineType^ get() { return m_lineType; }
			void set(LineType^ value) { m_lineType = value; }
		}*/

		property IPainter^ Owner
		{
			IPainter^ get() { return m_owner; }
			void set(IPainter^ value) { m_owner = value; }
		}

		property System::Drawing::Color LineColor
		{
			System::Drawing::Color get() { return m_color; }
			void set(System::Drawing::Color value) { m_color = value; }
		}

		property System::String^ LayerName
		{
			System::String^ get() { return m_strLayerName; }
			void set(System::String^ value) { m_strLayerName = value; }
		}

		property bool UseGroupItem
		{
			bool get() { return m_useGroupItem; }
			void set(bool value) { m_useGroupItem = value; }
		}

		property DXFViewer::ShapeGroupOption^ ShapeGroupOption
		{
			DXFViewer::ShapeGroupOption^ get() { return m_shpGroupOption; }
			void set(DXFViewer::ShapeGroupOption^ value) { m_shpGroupOption = value; }
		}

		property bool VisibleGroup
		{
			bool get() { return m_isVisibleGroup; }
			void set(bool value) { m_isVisibleGroup = value; }
		}

	protected:
		System::String^ m_strLayerName;
		System::Collections::ArrayList^ m_listObject;
		System::Drawing::Color m_color;
		LineType^ m_lineType;
		bool m_isHidden;
		bool m_isLock;
		bool m_isFrozen;
		IPainter^ m_owner;
		bool m_isVisibleGroup;

		int m_nGroupItemDistance;
		// 가까운 거리에 있는 Item들을 Group으로 묶을 것인가?
		bool m_useGroupItem;
		System::Collections::ArrayList^ m_listGroup;
		DXFViewer::ShapeGroupOption^ m_shpGroupOption;
	};
}
