using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
	// just like in c++, we could write (we could emmit) our own IL and bake it into our process at runtime and use it
	// build our own newable delegate
	public static class FastObjectAllocator<T> where T : new()
	{
		public static Func<T> mObjectCreator = null;

		static FastObjectAllocator()
		{
			if (mObjectCreator == null)
			{
				Type objectType = typeof(T);
				ConstructorInfo defaultCtor = objectType.GetConstructor(new Type[] { });

				DynamicMethod dynMethod = new DynamicMethod(
					name: string.Format("_{0:N}", Guid.NewGuid()),
					returnType: objectType,
					parameterTypes: null
				);

				ILGenerator il = dynMethod.GetILGenerator();
				il.Emit(OpCodes.Newobj, defaultCtor);
				il.Emit(OpCodes.Ret);

				mObjectCreator = dynMethod.CreateDelegate(typeof(Func<T>)) as Func<T>;
			}
		}

		public static T New()
		{
			return mObjectCreator();
		}
	}
}
