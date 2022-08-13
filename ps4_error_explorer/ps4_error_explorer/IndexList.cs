using System;

namespace ps4_error_explorer
{
	internal class IndexList
	{
		public IndexList(int num)
		{
			this.num = 0;
			this.list = new int[num];
		}

		public void Add(int index)
		{
			this.list[this.num] = index;
			this.num++;
		}

		public void Clear()
		{
			this.num = 0;
		}

		public int Get(int index)
		{
			return this.list[index];
		}

		public int GetCount()
		{
			return this.num;
		}

		private int[] list;

		private int num;
	}
}
