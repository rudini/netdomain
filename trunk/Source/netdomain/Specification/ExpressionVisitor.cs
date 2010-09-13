//-------------------------------------------------------------------------------
// <copyright file="ExpressionVisitor.cs" company="bbv Software Services AG">
//   Copyright (c) 2010 Roger Rudin, bbv Software Services AG
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//
// Contains parts from "How to: Implement an Expression Tree Visitor" 
// of MSDN Visual Studio 2008 Developer Center
// published on http://msdn.microsoft.com/en-us/library/bb882521.aspx.
// </copyright>
//------------------------------------------------------------------------------

namespace netdomain.Specification
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;

    /// <summary>
    /// Implements an expression tree visitor. 
    /// This class is designed to be inherited to create more specialized classes whose functionality requires traversing, 
    /// examining or copying an expression tree.
    /// <remarks>The implementation has been taken from MSDN Visual Studio 2008 Developer Center <a href="http://msdn.microsoft.com/en-us/library/bb882521.aspx"></a>/></remarks>
    /// </summary>
    public abstract class ExpressionVisitor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionVisitor"/> class.
        /// </summary>
        protected ExpressionVisitor()
        {
        }

        /// <summary>
        /// Dispatches the expression it is passed to one of the more specialized visitor methods in the class,  based on the type of the expression. 
        /// The specialized visitor methods visit the sub-tree of the expression they are passed. 
        /// If a sub-expression changes after it has been visited, for example by an overriding method in a derived class, 
        /// the specialized visitor methods create a new expression that includes the changes in the sub-tree. 
        /// Otherwise, they return the expression that they were passed. 
        /// This recursive behavior enables a new expression tree to be built that either is the same as or a modified version 
        /// of the original expression that was passed to Visit. 
        /// </summary>
        /// <param name="exp">The expression to visit.</param>
        /// <returns>If a sub-expression changes after it has been visited, for example by an overriding method in a derived class, 
        /// the specialized visitor methods create a new expression that includes the changes in the sub-tree. 
        /// Otherwise, they return the expression that they were passed. 
        /// </returns>
        protected virtual Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                return exp;
            }

            switch (exp.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return this.VisitUnary((UnaryExpression)exp);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return this.VisitBinary((BinaryExpression)exp);
                case ExpressionType.TypeIs:
                    return this.VisitTypeIs((TypeBinaryExpression)exp);
                case ExpressionType.Conditional:
                    return this.VisitConditional((ConditionalExpression)exp);
                case ExpressionType.Constant:
                    return this.VisitConstant((ConstantExpression)exp);
                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression)exp);
                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression)exp);
                case ExpressionType.Call:
                    return this.VisitMethodCall((MethodCallExpression)exp);
                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression)exp);
                case ExpressionType.New:
                    return this.VisitNew((NewExpression)exp);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return this.VisitNewArray((NewArrayExpression)exp);
                case ExpressionType.Invoke:
                    return this.VisitInvocation((InvocationExpression)exp);
                case ExpressionType.MemberInit:
                    return this.VisitMemberInit((MemberInitExpression)exp);
                case ExpressionType.ListInit:
                    return this.VisitListInit((ListInitExpression)exp);
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
            }
        }

        /// <summary>
        /// Dispatches the <see cref="MemberBinding"/> it is passed to one of the more specialized visitor methods in the class,  based on the type of the member binding. 
        /// The specialized visitor methods visit the sub-tree of the expression they are passed.
        /// This method is called from the Visit method. 
        /// </summary>
        /// <param name="binding">The member binding.</param>
        /// <returns>If a sub-expression changes after it has been visited, for example by an overriding method in a derived class, 
        /// the specialized visitor methods create a new expression that includes the changes in the sub-tree. 
        /// Otherwise, they return the expression that they were passed.</returns>
        protected virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return this.VisitMemberAssignment((MemberAssignment)binding);
                case MemberBindingType.MemberBinding:
                    return this.VisitMemberMemberBinding((MemberMemberBinding)binding);
                case MemberBindingType.ListBinding:
                    return this.VisitMemberListBinding((MemberListBinding)binding);
                default:
                    throw new Exception(string.Format("Unhandled binding type '{0}'", binding.BindingType));
            }
        }

        /// <summary>
        /// Visits the element initializer.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        /// <returns></returns>
        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            ReadOnlyCollection<Expression> arguments = this.VisitExpressionList(initializer.Arguments);

            return arguments != initializer.Arguments ? Expression.ElementInit(initializer.AddMethod, arguments) : initializer;
        }

        /// <summary>
        /// Visits the <see cref="UnaryExpression"/> expression by
        /// calling <see cref="Visit"/> with the <see cref="UnaryExpression.Operand"/> expression.
        /// </summary>
        /// <param name="u">The <see cref="UnaryExpression"/>.</param>
        /// <returns></returns>
        protected virtual Expression VisitUnary(UnaryExpression u)
        {
            var operand = this.Visit(u.Operand);

            return operand != u.Operand ? Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method) : u;
        }

        /// <summary>
        /// Visits the <see cref="BinaryExpression"/> by calling
        /// <see cref="Visit"/> with the <see cref="BinaryExpression.Left"/>,
        /// <see cref="BinaryExpression.Right"/> and <see cref="BinaryExpression.Conversion"/>
        /// expressions.
        /// </summary>
        /// <param name="b">The <see cref="BinaryExpression"/>.</param>
        /// <returns></returns>
        protected virtual Expression VisitBinary(BinaryExpression b)
        {
            var left = this.Visit(b.Left);
            var right = this.Visit(b.Right);
            var conversion = this.Visit(b.Conversion);
            
            if (left != b.Left || right != b.Right || conversion != b.Conversion)
            {
                if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
                {
                    return Expression.Coalesce(left, right, conversion as LambdaExpression);
                }
                
                return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
            }

            return b;
        }

        /// <summary>
        /// Visits the <see cref="TypeBinaryExpression"/> by calling
        /// <see cref="Visit"/> with the <see cref="TypeBinaryExpression.Expression"/>
        /// expression.
        /// </summary>
        /// <param name="b">The <see cref="TypeBinaryExpression"/>.</param>
        /// <returns></returns>
        protected virtual Expression VisitTypeIs(TypeBinaryExpression b)
        {
            var expr = this.Visit(b.Expression);

            return expr != b.Expression ? Expression.TypeIs(expr, b.TypeOperand) : b;
        }

        /// <summary>
        /// Visits the <see cref="ConstantExpression"/>, by default returning the
        /// same <see cref="ConstantExpression"/> without further behavior.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        protected virtual Expression VisitConstant(ConstantExpression c)
        {
            return c;
        }

        /// <summary>
        /// Visits the <see cref="ConditionalExpression"/> by calling
        /// <see cref="Visit"/> with the <see cref="ConditionalExpression.Test"/>,
        /// <see cref="ConditionalExpression.IfTrue"/> and <see cref="ConditionalExpression.IfFalse"/>
        /// expressions.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        protected virtual Expression VisitConditional(ConditionalExpression c)
        {
            var test = this.Visit(c.Test);
            var ifTrue = this.Visit(c.IfTrue);
            var ifFalse = this.Visit(c.IfFalse);

            if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse)
            {
                return Expression.Condition(test, ifTrue, ifFalse);
            }

            return c;
        }

        /// <summary>
        /// Visits the <see cref="ParameterExpression"/> returning it
        /// by default without further behavior.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        protected virtual Expression VisitParameter(ParameterExpression p)
        {
            return p;
        }

        /// <summary>
        /// Visits the <see cref="MemberExpression"/> by calling
        /// <see cref="Visit"/> with the <see cref="MemberExpression.Expression"/>
        /// expression.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        protected virtual Expression VisitMemberAccess(MemberExpression m)
        {
            var exp = this.Visit(m.Expression);

            return exp != m.Expression ? Expression.MakeMemberAccess(exp, m.Member) : m;
        }

        /// <summary>
        /// Visits the <see cref="MethodCallExpression"/> by calling
        /// <see cref="Visit"/> with the <see cref="MethodCallExpression.Object"/> expression,
        /// and then <see cref="VisitExpressionList"/> with the <see cref="MethodCallExpression.Arguments"/>.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        protected virtual Expression VisitMethodCall(MethodCallExpression m)
        {
            var obj = this.Visit(m.Object);

            IEnumerable<Expression> args = this.VisitExpressionList(m.Arguments);

            if (obj != m.Object || args != m.Arguments)
            {
                return Expression.Call(obj, m.Method, args);
            }

            return m;
        }

        /// <summary>
        /// Visits the <see cref="ReadOnlyCollection{Expression}"/> by iterating
        /// the list and visiting each <see cref="Expression"/> in it.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <returns></returns>
        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Expression> list = null;

            for (int i = 0, n = original.Count; i < n; i++)
            {
                var p = this.Visit(original[i]);

                if (list != null)
                {
                    list.Add(p);
                }

                else if (p != original[i])
                {
                    list = new List<Expression>(n);

                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }

                    list.Add(p);
                }
            }

            return list != null ? list.AsReadOnly() : original;
        }

        /// <summary>
        /// Visits the <see cref="MemberAssignment"/> by calling 
        /// <see cref="Visit"/> with the <see cref="MemberAssignment.Expression"/> expression.
        /// </summary>
        /// <param name="assignment"></param>
        /// <returns></returns>
        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            Expression e = this.Visit(assignment.Expression);

            if (e != assignment.Expression)
            {
                return Expression.Bind(assignment.Member, e);
            }

            return assignment;
        }

        /// <summary>
        /// Visits the <see cref="MemberMemberBinding"/> by calling 
        /// <see cref="VisitBindingList"/> with the <see cref="MemberMemberBinding.Bindings"/>.
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(binding.Bindings);

            if (bindings != binding.Bindings)
            {
                return Expression.MemberBind(binding.Member, bindings);
            }

            return binding;
        }

        /// <summary>
        /// Visits the <see cref="MemberListBinding"/> by calling 
        /// <see cref="VisitElementInitializerList"/> with the 
        /// <see cref="MemberListBinding.Initializers"/>.
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(binding.Initializers);

            if (initializers != binding.Initializers)
            {
                return Expression.ListBind(binding.Member, initializers);
            }

            return binding;
        }

        /// <summary>
        /// Visits the <see cref="ReadOnlyCollection{MemberBinding}"/> by 
        /// calling <see cref="VisitBinding"/> for each <see cref="MemberBinding"/> in the 
        /// collection.
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            List<MemberBinding> list = null;

            for (int i = 0, n = original.Count; i < n; i++)
            {
                MemberBinding b = this.VisitBinding(original[i]);

                if (list != null)
                {
                    list.Add(b);
                }

                else if (b != original[i])
                {
                    list = new List<MemberBinding>(n);

                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }

                    list.Add(b);
                }
            }

            if (list != null)
                return list;

            return original;
        }

        /// <summary>
        /// Visits the <see cref="ReadOnlyCollection{ElementInit}"/> by 
        /// calling <see cref="VisitElementInitializer"/> for each 
        /// <see cref="ElementInit"/> in the collection.
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            List<ElementInit> list = null;

            for (int i = 0, n = original.Count; i < n; i++)
            {
                ElementInit init = this.VisitElementInitializer(original[i]);

                if (list != null)
                {
                    list.Add(init);
                }

                else if (init != original[i])
                {
                    list = new List<ElementInit>(n);

                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }

                    list.Add(init);
                }
            }

            if (list != null)
                return list;

            return original;
        }

        /// <summary>
        /// Visits the <see cref="LambdaExpression"/> by calling 
        /// <see cref="Visit"/> with the <see cref="LambdaExpression.Body"/> expression.
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns></returns>
        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            Expression body = this.Visit(lambda.Body);

            if (body != lambda.Body)
            {
                return Expression.Lambda(lambda.Type, body, lambda.Parameters);
            }

            return lambda;
        }

        /// <summary>
        /// Visits the <see cref="NewExpression"/> by calling 
        /// <see cref="VisitExpressionList"/> with the <see cref="NewExpression.Arguments"/> 
        /// expressions.
        /// </summary>
        /// <param name="nex"></param>
        /// <returns></returns>
        protected virtual NewExpression VisitNew(NewExpression nex)
        {
            IEnumerable<Expression> args = this.VisitExpressionList(nex.Arguments);

            if (args != nex.Arguments)
            {
                if (nex.Members != null)
                {
                    return Expression.New(nex.Constructor, args, nex.Members);
                }
                
                return Expression.New(nex.Constructor, args);
            }

            return nex;
        }

        /// <summary>
        /// Visits the <see cref="MemberInitExpression"/> by calling 
        /// <see cref="VisitNew"/> with the <see cref="MemberInitExpression.NewExpression"/> 
        /// expression, then <see cref="VisitBindingList"/> with the 
        /// <see cref="MemberInitExpression.Bindings"/>.
        /// </summary>
        protected virtual Expression VisitMemberInit(MemberInitExpression init)
        {
            NewExpression n = this.VisitNew(init.NewExpression);

            IEnumerable<MemberBinding> bindings = this.VisitBindingList(init.Bindings);

            if (n != init.NewExpression || bindings != init.Bindings)
            {
                return Expression.MemberInit(n, bindings);
            }

            return init;
        }

        /// <summary>
        /// Visits the <see cref="ListInitExpression"/> by calling 
        /// <see cref="VisitNew"/> with the <see cref="ListInitExpression.NewExpression"/> 
        /// expression, and then <see cref="VisitElementInitializerList"/> with the 
        /// <see cref="ListInitExpression.Initializers"/>.
        /// </summary>
        /// <param name="init"></param>
        /// <returns></returns>
        protected virtual Expression VisitListInit(ListInitExpression init)
        {
            NewExpression n = this.VisitNew(init.NewExpression);

            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(init.Initializers);

            if (n != init.NewExpression || initializers != init.Initializers)
            {
                return Expression.ListInit(n, initializers);
            }

            return init;
        }

        /// <summary>
        /// Visits the <see cref="NewArrayExpression"/> by calling 
        /// <see cref="VisitExpressionList"/> with the <see cref="NewArrayExpression.Expressions"/> 
        /// expressions.
        /// </summary>
        /// <param name="na"></param>
        /// <returns></returns>
        protected virtual Expression VisitNewArray(NewArrayExpression na)
        {
            IEnumerable<Expression> exprs = this.VisitExpressionList(na.Expressions);

            if (exprs != na.Expressions)
            {
                if (na.NodeType == ExpressionType.NewArrayInit)
                {
                    return Expression.NewArrayInit(na.Type.GetElementType(), exprs);
                }
                
                return Expression.NewArrayBounds(na.Type.GetElementType(), exprs);
            }

            return na;
        }

        /// <summary>
        /// Visits the <see cref="InvocationExpression"/> by calling 
        /// <see cref="VisitExpressionList"/> with the <see cref="InvocationExpression.Arguments"/> 
        /// expressions.
        /// </summary>
        /// <param name="iv"></param>
        /// <returns></returns>
        protected virtual Expression VisitInvocation(InvocationExpression iv)
        {
            IEnumerable<Expression> args = this.VisitExpressionList(iv.Arguments);

            Expression expr = this.Visit(iv.Expression);

            if (args != iv.Arguments || expr != iv.Expression)
            {
                return Expression.Invoke(expr, args);
            }

            return iv;
        }
    }
}
