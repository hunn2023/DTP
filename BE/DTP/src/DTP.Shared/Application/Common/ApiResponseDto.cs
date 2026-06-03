using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Shared.Application.Common
{
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; }

        public string? Message { get; set; }

        public T? Data { get; set; }

        public List<ErrorDto> Errors { get; set; } = new();

        public static ApiResponseDto<T> Ok(T data, string? message = null)
        {
            return new ApiResponseDto<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponseDto<T> Fail(string message)
        {
            return new ApiResponseDto<T>
            {
                Success = false,
                Message = message
            };
        }

        public static ApiResponseDto<T> Fail(List<ErrorDto> errors, string? message = null)
        {
            return new ApiResponseDto<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }
}
