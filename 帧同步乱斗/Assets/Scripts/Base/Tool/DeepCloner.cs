using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;



/// <summary>
/// 深拷贝器
/// </summary>
public class DeepCloner : Singleton<DeepCloner>
{
    public T DeepCopy<T>(T obj)
    {
        if (obj is string || obj.GetType().IsValueType || obj is GameObject || obj is MonoBehaviour)
        {
            return obj;
        }
        object rslt = Activator.CreateInstance(obj.GetType());
        if (rslt is IList)
        {
            IList list = rslt as IList;
            foreach (object item in obj as IList)
            {
                list.Add(DeepCopy(item));
            }
        }
        else
        {
            var fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            foreach (var item in fields)
            {
                try
                {
                    item.SetValue(rslt, DeepCopy(item.GetValue(obj)));
                }
                catch (Exception ex)
                {
                    string err = ex.Message;
                }
            }
        }
        return (T)rslt;
    }



    #region 表达式树，有问题
    private Dictionary<Type, Delegate> cloneFuncDic = new();
    /// <summary>
    /// 深拷贝,只有一层，相互引用的问题
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private T DeepClone<T>(T obj) where T : class
    {
        Type type = obj.GetType();
        if (cloneFuncDic.ContainsKey(type) == false)
        {
            cloneFuncDic.Add(type, GetCopyDelegate(type));
        }
        return cloneFuncDic[type].DynamicInvoke(obj) as T;
    }

    private Delegate GetCopyDelegate(Type type)
    {
        ParameterExpression parameterExpression = Expression.Parameter(type, "p");
        MemberInitExpression memberInitExpression = GetCopyExpression(type, parameterExpression);

        Type generic = typeof(Func<,>);
        Type[] typeArgs = { type, type };
        Type constructed = generic.MakeGenericType(typeArgs);
        LambdaExpression lambda = Expression.Lambda(constructed, memberInitExpression, new ParameterExpression[] { parameterExpression });
        return lambda.Compile();
    }
    /// <summary>
    /// 复制一个类表达式
    /// </summary>
    /// <param name="type"></param>
    /// <param name="originParameterExpression"></param>
    /// <returns></returns>
    private MemberInitExpression GetCopyExpression(Type type, Expression originParameterExpression)
    {
        List<MemberBinding> memberBindingList = new List<MemberBinding>();

        foreach (var fieldInfo in type.GetFields())
        {
            if (!fieldInfo.CanWrite()) continue;
            MemberExpression fieldExpression = Expression.Field(originParameterExpression, fieldInfo);
            //值类型直接绑定
            if (fieldInfo.FieldType == typeof(string) || fieldInfo.FieldType.IsValueType || fieldInfo.FieldType == typeof(GameObject))
            {
                MemberBinding memberBinding = Expression.Bind(fieldInfo, fieldExpression);
                memberBindingList.Add(memberBinding);
            }
            //否则递归
            else
            {
                Expression copyFieldExpression = GetCopyExpression(fieldInfo.FieldType, fieldExpression); //这里的类型应该是动态的，有问题
                MemberBinding memberBinding = Expression.Bind(fieldInfo, copyFieldExpression);
                memberBindingList.Add(memberBinding);
            }
        }

        MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(type), memberBindingList.ToArray());
        return memberInitExpression;
    }

    private static Delegate GetFunc(Type inType, Type outType)
    {
        ParameterExpression parameterExpression = Expression.Parameter(inType, "p");
        List<MemberBinding> memberBindingList = new List<MemberBinding>();

        foreach (var item in outType.GetProperties())
        {
            if (!item.CanWrite) continue;
            MemberExpression property = Expression.Property(parameterExpression, item);
            MemberBinding memberBinding = Expression.Bind(item, property);
            memberBindingList.Add(memberBinding);
        }

        foreach (var item in outType.GetFields())
        {
            if (!item.CanWrite()) continue;
            MemberExpression property = Expression.Field(parameterExpression, item);

            MemberBinding memberBinding = Expression.Bind(item, property);
            memberBindingList.Add(memberBinding);
        }



        MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(outType), memberBindingList.ToArray());

        Type generic = typeof(Func<,>);
        Type[] typeArgs = { inType, outType };
        Type constructed = generic.MakeGenericType(typeArgs);
        LambdaExpression lambda = Expression.Lambda(constructed, memberInitExpression, new ParameterExpression[] { parameterExpression });
        Delegate d = lambda.Compile();
        return d;
    }
    #endregion

}


