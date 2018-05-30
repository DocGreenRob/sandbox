using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
	public static class utils
	{
		public static Func<Arg, Ret> Memoize<Arg, Ret>(this Func<Arg, Ret> functor)
		{
			var memo_table = new Dictionary<Arg, Ret>();

			return (arg0) =>
			{
				Ret func_return_val;

				if (!memo_table.TryGetValue(arg0, out func_return_val))
				{
					func_return_val = functor(arg0);
					memo_table.Add(arg0, func_return_val);
				}

				return func_return_val;
			};
		}
	}
}
