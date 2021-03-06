﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Text.Json;
using Google.Protobuf;
using Microsoft.Azure.Functions.Worker.Definition;
using Microsoft.Azure.WebJobs.Script.Grpc.Messages;

namespace Microsoft.Azure.Functions.Worker
{
    internal static class RpcExtensions
    {
        public static TypedData ToRpc(this object value)
        {
            TypedData typedData = new TypedData();
            if (value == null)
            {
                return typedData;
            }
            if (value is byte[] arr)
            {
                typedData.Bytes = ByteString.CopyFrom(arr);
            }
            else if (value is string str)
            {
                typedData.String = str;
            }
            else if (value is HttpResponseData response)
            {
                typedData = response.ToRpcHttp();
            }
            else if (value.GetType().IsArray)
            {
                typedData = ToRpcCollection(value);
            }
            else
            {
                typedData = value.ToRpcDefault();
            }

            return typedData;
        }

        internal static TypedData ToRpcDefault(this object value)
        {
            // attempt POCO / array of pocos
            TypedData typedData = new TypedData();
            try
            {
                typedData.Json = JsonSerializer.Serialize(value);
            }
            catch
            {
                typedData.String = value.ToString();
            }

            return typedData;
        }

        public static TypedData ToRpcCollection(this object value)
        {
            TypedData typedData;
            if (value is byte[][] arrBytes)
            {
                typedData = arrBytes.ToRpcByteArray();
            }
            else if (value is string[] arrStr)
            {
                typedData = arrStr.ToRpcStringArray();
            }
            else if (value is double[] arrDouble)
            {
                typedData = arrDouble.ToRpcDoubleArray();
            }
            else if (value is long[] arrLong)
            {
                typedData = arrLong.ToRpcLongArray();
            }
            else
            {
                typedData = value.ToRpcDefault();
            }

            return typedData;
        }

        internal static TypedData ToRpcHttp(this HttpResponseData response)
        {
            var http = new RpcHttp()
            {
                StatusCode = ((int)response.StatusCode).ToString()
            };

            if (response.Body != null)
            {
                http.Body = response.Body.ToRpc();
            }
            else
            {
                // TODO: Is this correct? Passing a null body causes the entire
                //       response to become the body in functions. Need to investigate.
                http.Body = string.Empty.ToRpc();
            }

            if (response.Headers != null)
            {
                foreach (var pair in response.Headers)
                {
                    // maybe check or validate that the headers make sense?
                    http.Headers.Add(pair.Key.ToLowerInvariant(), pair.Value.ToString());
                }
            }
            var typedData = new TypedData
            {
                Http = http
            };
            return typedData;
        }

        internal static TypedData ToRpcByteArray(this byte[][] arrBytes)
        {
            TypedData typedData = new TypedData();
            CollectionBytes collectionBytes = new CollectionBytes();
            foreach (byte[] element in arrBytes)
            {
                if (element != null)
                {
                    collectionBytes.Bytes.Add(ByteString.CopyFrom(element));
                }
            }
            typedData.CollectionBytes = collectionBytes;

            return typedData;
        }

        internal static TypedData ToRpcStringArray(this string[] arrString)
        {
            TypedData typedData = new TypedData();
            CollectionString collectionString = new CollectionString();
            foreach (string element in arrString)
            {
                if (!string.IsNullOrEmpty(element))
                {
                    collectionString.String.Add(element);
                }
            }
            typedData.CollectionString = collectionString;

            return typedData;
        }

        internal static TypedData ToRpcDoubleArray(this double[] arrDouble)
        {
            TypedData typedData = new TypedData();
            CollectionDouble collectionDouble = new CollectionDouble();
            foreach (double element in arrDouble)
            {
                collectionDouble.Double.Add(element);
            }
            typedData.CollectionDouble = collectionDouble;

            return typedData;
        }

        internal static TypedData ToRpcLongArray(this long[] arrLong)
        {
            TypedData typedData = new TypedData();
            CollectionSInt64 collectionLong = new CollectionSInt64();
            foreach (long element in arrLong)
            {
                collectionLong.Sint64.Add(element);
            }
            typedData.CollectionSint64 = collectionLong;

            return typedData;
        }

        internal static FunctionMetadata ToFunctionMetadata(this FunctionLoadRequest loadRequest) => new GrpcFunctionMetadata(loadRequest);

        internal static RpcException? ToRpcException(this Exception exception)
        {
            if (exception == null)
            {
                return null;
            }

            return new RpcException
            {
                Message = exception.Message,
                Source = exception.Source,
                StackTrace = exception.StackTrace
            };
        }
    }
}
