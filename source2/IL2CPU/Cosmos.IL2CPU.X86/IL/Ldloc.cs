using System;
using Cosmos.IL2CPU.ILOpCodes;
using CPUx86 = Cosmos.Compiler.Assembler.X86;
using Cosmos.Compiler.Assembler;

namespace Cosmos.IL2CPU.X86.IL
{
	[Cosmos.IL2CPU.OpCode(ILOpCode.Code.Ldloc)]
	public class Ldloc : ILOp
	{
		public Ldloc(Cosmos.Compiler.Assembler.Assembler aAsmblr)
			: base(aAsmblr)
		{
		}

		public override void Execute(MethodInfo aMethod, ILOpCode aOpCode)
		{
			var xOpVar = (OpVar)aOpCode;
			var xVar = aMethod.MethodBase.GetMethodBody().LocalVariables[xOpVar.Value];
			var xStackCount = GetStackCountForLocal(aMethod, xVar);
			var xEBPOffset = ((int)GetEBPOffsetForLocal(aMethod, xOpVar.Value));
			var xSize = SizeOfType(xVar.LocalType);
            new Comment("EBPOffset = " + xEBPOffset); if (xStackCount > 1)
			{
				for (int i = 0; i < xStackCount; i++)
				{
					new CPUx86.Move { DestinationReg = CPUx86.Registers.EAX, SourceReg = CPUx86.Registers.EBP, SourceIsIndirect = true, SourceDisplacement = (int)(0 - (xEBPOffset + (i * 4))) };
					new CPUx86.Push { DestinationReg = CPUx86.Registers.EAX };
				}
			}
			else
			{
				new CPUx86.Xor { DestinationReg = CPUx86.Registers.EAX, SourceReg = CPUx86.Registers.EAX };

				switch (xSize)
				{
					case 1:
						{
							new CPUx86.Move { DestinationReg = CPUx86.Registers.AL, SourceReg = CPUx86.Registers.EBP, SourceIsIndirect = true, SourceDisplacement = 0 - xEBPOffset };
							break;
						}
					case 2:
						{
              new CPUx86.Move { DestinationReg = CPUx86.Registers.AX, SourceReg = CPUx86.Registers.EBP, SourceIsIndirect = true, SourceDisplacement = 0 - xEBPOffset };

							break;
						}
					case 4:
						{
              new CPUx86.Move { DestinationReg = CPUx86.Registers.EAX, SourceReg = CPUx86.Registers.EBP, SourceIsIndirect = true, SourceDisplacement = 0 - xEBPOffset };
							break;
						}
				}
				new CPUx86.Push { DestinationReg = CPUx86.Registers.EAX };
			}
			Assembler.Stack.Push((int)xSize, xVar.LocalType);
		}


		// using System;
		// using System.IO;
		// using Cosmos.IL2CPU.X86;
		// 
		// 
		// using CPU = Cosmos.Compiler.Assembler.X86;
		// using Cosmos.IL2CPU.Compiler;
		// 
		// namespace Cosmos.IL2CPU.IL.X86 {
		// 	[OpCode(OpCodeEnum.Ldloc)]
		// 	public class Ldloc: Op {
		// 		private MethodInformation.Variable mLocal;
		// 		protected void SetLocalIndex(int aIndex, MethodInformation aMethodInfo) {
		// 			mLocal = aMethodInfo.Locals[aIndex];
		// 		}
		// 		public Ldloc(MethodInformation aMethodInfo, int aIndex)
		// 			: base(null, aMethodInfo) {
		// 			SetLocalIndex(aIndex, aMethodInfo);
		// 		}
		// 
		// 		public Ldloc(ILReader aReader, MethodInformation aMethodInfo)
		// 			: base(aReader, aMethodInfo) {
		// 			SetLocalIndex(aReader.OperandValueInt32, aMethodInfo);
		// 			//VariableDefinition xVarDef = aReader.Operand as VariableDefinition;
		// 			//if (xVarDef != null) {
		// 			//    SetLocalIndex(xVarDef.Index, aMethodInfo);
		// 			//}
		// 		}
		// 
		// 		public sealed override void DoAssemble() {
		// 			Ldloc(Assembler, mLocal, GetService<IMetaDataInfoService>().SizeOfType(mLocal.VariableType));
		// 		}
		// 	}
		// }

	}
}