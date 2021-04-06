using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;

namespace Hacked.Wpf.Helpers
{
    public class CustomFilterDescriptor : FilterDescriptorBase
    {
        private static readonly ConstantExpression TrueExpression = Expression.Constant(true);
        private readonly CompositeFilterDescriptor _compositeFilterDesriptor;
        private string _filterValue;

        public CustomFilterDescriptor(IEnumerable<GridViewColumn> columns)
        {
            _compositeFilterDesriptor = new CompositeFilterDescriptor();
            _compositeFilterDesriptor.LogicalOperator = FilterCompositionLogicalOperator.Or;

            foreach (GridViewDataColumn column in columns)
            {
                _compositeFilterDesriptor.FilterDescriptors.Add(CreateFilterForColumn(column));
            }
        }

        public string FilterValue
        {
            get => _filterValue;
            set
            {
                if (_filterValue != value)
                {
                    _filterValue = value;
                    UpdateCompositeFilterValues();
                    OnPropertyChanged("FilterValue");
                }
            }
        }

        protected override Expression CreateFilterExpression(ParameterExpression parameterExpression)
        {
            Expression expression = TrueExpression;
            if (!string.IsNullOrEmpty(FilterValue))
            {
                try
                {
                    expression = _compositeFilterDesriptor.CreateFilterExpression(parameterExpression);
                }
                catch
                {
                }
            }

            return expression;
        }

        private IFilterDescriptor CreateFilterForColumn(GridViewDataColumn column)
        {
            var filterOperator = GetFilterOperatorForType(column.DataType);
            return new FilterDescriptor(column.UniqueName, filterOperator, _filterValue)
            {
                MemberType = column.DataType
            };
        }

        private static FilterOperator GetFilterOperatorForType(Type dataType)
        {
            return dataType == typeof(string) ? FilterOperator.Contains : FilterOperator.IsEqualTo;
        }

        private static object DefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        private void UpdateCompositeFilterValues()
        {
            foreach (FilterDescriptor descriptor in _compositeFilterDesriptor.FilterDescriptors)
            {
                object convertedValue = DefaultValue(descriptor.MemberType);

                try
                {
                    convertedValue = Convert.ChangeType(FilterValue, descriptor.MemberType, CultureInfo.InvariantCulture);
                }
                catch
                {
                    convertedValue = OperatorValueFilterDescriptorBase.UnsetValue;
                }

                if (descriptor.MemberType.IsAssignableFrom(typeof(DateTime)))
                {
                    DateTime date;
                    if (DateTime.TryParse(FilterValue, out date))
                    {
                        convertedValue = date;
                    }
                }

                descriptor.Value = convertedValue;
            }
        }
    }
}