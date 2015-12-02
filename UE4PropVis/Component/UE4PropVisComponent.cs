using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.ComponentInterfaces;
using Microsoft.VisualStudio.Debugger.Evaluation;

using UE4PropVis.Core;
using UE4PropVis.Core.EE;


namespace UE4PropVis
{
	static class Impl
	{
		public static ObjectContext.Factory ObjCtxFactory = new EvaluatedObjectContext.MyFactory();
	};

    class UE4PropVisComponent : IDkmCustomVisualizer
    {
        static UE4PropVisComponent()
        {
			UE4VisualizerRegistrar.Register< UObjectVisualizer.Factory >(Guids.Visualizer.UObject);
			//UE4VisualizerRegistrar.Register< PropertyListVisualizer.Factory >(Guids.Visualizer.PropertyList);
		}

		void OnVisualizerMatchFailed(DkmVisualizedExpression expression, out DkmEvaluationResult result)
		{
			result = DkmFailedEvaluationResult.Create(
				expression.InspectionContext,
				expression.StackFrame,
				Utility.GetExpressionName(expression),
				Utility.GetExpressionFullName(expression),
				String.Format("UE4PropVis: No visualizer is registered for VisualizerId {0}", expression.VisualizerId),
				DkmEvaluationResultFlags.Invalid,
				null
				);
		}

		void IDkmCustomVisualizer.EvaluateVisualizedExpression(DkmVisualizedExpression expression, out DkmEvaluationResult resultObject)
		{
			// Sanity check to confirm this is only being invoked for UObject types. @TODO: Remove eventually.
			// Believe this method is only invoked on DkmRootVisualizedExpression instances, not children.
			Debug.Assert(expression.VisualizerId == Guids.Visualizer.UObject);

			UE4Visualizer visualizer = null;
			bool result = UE4VisualizerRegistrar.TryCreateVisualizer(expression, out visualizer);
			if(!result)
			{
				OnVisualizerMatchFailed(expression, out resultObject);
				return;
			}

			// Evaluate the visualization result
			DkmEvaluationResult eval = visualizer.EvaluationResult;
			resultObject = eval;

			// Associate the visualizer with the expression
			expression.SetDataItem<ExpressionDataItem>(
				DkmDataCreationDisposition.CreateAlways,
				new ExpressionDataItem(visualizer)
				);
		}

        void IDkmCustomVisualizer.GetChildren(DkmVisualizedExpression expression, int initialRequestSize, DkmInspectionContext inspectionContext, out DkmChildVisualizedExpression[] initialChildren, out DkmEvaluationResultEnumContext enumContext)
        {
			var data_item = expression.GetDataItem<ExpressionDataItem>();
			var visualizer = data_item.Visualizer;
			Debug.Assert(visualizer != null);

			visualizer.PrepareExpansion(out enumContext);
			initialChildren = new DkmChildVisualizedExpression[0];
		}

        void IDkmCustomVisualizer.GetItems(DkmVisualizedExpression expression, DkmEvaluationResultEnumContext enumContext, int startIndex, int count, out DkmChildVisualizedExpression[] items)
        {
			var data_item = expression.GetDataItem<ExpressionDataItem>();
			var visualizer = data_item.Visualizer;
			Debug.Assert(visualizer != null);

			visualizer.GetChildItems(enumContext, startIndex, count, out items);
        }

        string IDkmCustomVisualizer.GetUnderlyingString(DkmVisualizedExpression visualizedExpression)
        {
            throw new NotImplementedException();
        }

        void IDkmCustomVisualizer.SetValueAsString(DkmVisualizedExpression visualizedExpression, string value, int timeout, out string errorText)
        {
            throw new NotImplementedException();
        }

        void IDkmCustomVisualizer.UseDefaultEvaluationBehavior(DkmVisualizedExpression expression, out bool useDefaultEvaluationBehavior, out DkmEvaluationResult defaultEvaluationResult)
        {
			var data_item = expression.GetDataItem<ExpressionDataItem>();
			if (data_item != null)
			{
				Debug.Assert(data_item.Visualizer != null);

				if (data_item.Visualizer.WantsCustomExpansion)
				{
					useDefaultEvaluationBehavior = false;
					defaultEvaluationResult = null;
					return;
				}
			}

			// Don't need any special expansion, just delegate back to the default EE
			useDefaultEvaluationBehavior = true;

			// @TODO: Perhaps here too, if child expression there is no need to reevaluate?
			// Seems that this isn't accepted - the expansion just doesn't generate anything.
			// Not sure how the evaluation result differs from a default-generated one (after all we basically
			// just used the default one with very minor alterations) for the EE to reject it, but this
			// does kind of fit with what is mentioned in the docs.
			if (false)//expression.TagValue == DkmVisualizedExpression.Tag.ChildVisualizedExpression)
			{
				defaultEvaluationResult = ((DkmChildVisualizedExpression)expression).EvaluationResult;
			}
			else
			{
				defaultEvaluationResult = DefaultEE.DefaultEval(expression, false);
			}
		}
	}
}
