﻿using BusinessLayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApiClient
{
    class LoginTokenResult
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string userName { get; set; }

    }
    class Program
    {
        
        //static HttpClientHandler httpClientHandler = new HttpClientHandler()
        //{
        //    Credentials = new NetworkCredential("pradeeprao@gmail.com", "Test@123")
        //};
        static HttpClient client = new HttpClient();
        static LoginTokenResult tokenResult = new LoginTokenResult();
        static void Main(string[] args)
        {
            client.BaseAddress = new Uri("https://localhost:44388/");
            client.DefaultRequestHeaders.Accept.Clear();
            
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            bool runningStatus = true;

            while (runningStatus)
            {
                Console.WriteLine("Enter Following Option");
                Console.WriteLine("To Get Token Press T");
                Console.WriteLine("To Add an Employee Press C");
                Console.WriteLine("To Get List of Employee Press R");
                Console.WriteLine("To Update Press U");
                Console.WriteLine("To Delete and Employee Press D");
                Console.WriteLine("To Get Particular Employee Press P");
                Console.WriteLine("To Exit Press E");
                string enter = Console.ReadLine();

                switch (enter.ToUpper())
                {

                    case "C":
                        Employee employee = new Employee();
                        Console.WriteLine("Enter Employee Id ");
                        employee.id = Convert.ToInt32(Console.ReadLine());


                        Console.WriteLine("Enter Employee First Name");
                        employee.first_name = Console.ReadLine();

                        Console.WriteLine("Enter Emloyee Last Name");
                        employee.last_name = Console.ReadLine();

                        Console.WriteLine("Enter Employee Salary");
                        int sal;
                        int.TryParse(Console.ReadLine(), out sal);
                        employee.salary = sal;

                        Console.WriteLine("Enter Employee City");
                        employee.city = Console.ReadLine();

                        Uri uri = CreateEmployee(employee).GetAwaiter().GetResult();

                        Console.WriteLine("Location = {0}", uri.ToString());


                        break;
                    case "R":
                        //string path = @"http://localhost:60390/api/Employee";
                        //List<Employee> employees = GetEmployeesAsync(path).GetAwaiter().GetResult();
                        //ShowProduct(employees);

                        string str = GetAllEmployees().GetAwaiter().GetResult();
                        Console.WriteLine(str);

                        break;
                    case "P":
                        Console.WriteLine("Enter Employee Id ");

                        int id = Convert.ToInt32(Console.ReadLine());

                        string employeeString = GetEmployeeById(id).GetAwaiter().GetResult();

                        Console.WriteLine("Employee with Id  {0} is not Found", employeeString);

                        break;

                    case "U":
                        Employee employeeToUpdate = new Employee();
                        Console.WriteLine("Enter Employee Id To Update");
                        employeeToUpdate.id = Convert.ToInt32(Console.ReadLine());

                        Console.WriteLine("Enter Employee Salary");

                        int.TryParse(Console.ReadLine(), out sal);
                        employeeToUpdate.salary = sal;

                        Console.WriteLine("Enter Employee City");
                        employeeToUpdate.city = Console.ReadLine();

                        string updateMessage = UpdateEmployeeAsync(employeeToUpdate).GetAwaiter().GetResult();

                        Console.WriteLine("Location = {0}", updateMessage);
                        break;
                    case "D":
                        Console.WriteLine("Enter Employee Id To Delete ");
                        int idtoDelete = Convert.ToInt32(Console.ReadLine());

                        string message = DeleteEmployee(idtoDelete).GetAwaiter().GetResult();

                        Console.WriteLine("Status Code is {0}", message.ToString());

                        break;
                    case "T":
                        string toekn = GetToken().GetAwaiter().GetResult();
                        Console.WriteLine("token value = {0} ", toekn);

                        break;
                    case "E":
                    default:
                        runningStatus = false;
                        Console.WriteLine("Exiting Application");
                        Console.ReadLine();
                        break;

                }

            }
        }

        static async Task<string> GetToken()
        {
            HttpResponseMessage response = await client.PostAsync("Token",
                new StringContent(string.Format("grant_type=password&username={0}&password={1}",
            "pradeeprao@gmail.com", "Test@123", System.Text.Encoding.UTF8,
            "application/x-www-form-urlencoded")));

            string resultJSON = response.Content.ReadAsStringAsync().Result;
            tokenResult = JsonConvert.DeserializeObject<LoginTokenResult>(resultJSON);

            return tokenResult.access_token;
        }

        static async Task<string> DeleteEmployee(int id)
        {
            HttpResponseMessage responseMessage = await client.DeleteAsync($"api/Employee/{id}");

            return responseMessage.Content.ReadAsStringAsync().Result;
            // return responseMessage.StatusCode;

        }


        static async Task<Uri> CreateEmployee(Employee employee)
        {
            //HttpRequestMessage httpRequestMessage = new HttpRequestMessage(new HttpMethod("POST"), $"api/Employee/");

            //httpRequestMessage.Content = employee;


            HttpResponseMessage responseMessage = await client.PostAsXmlAsync("api/Employee", employee);

            //if(responseMessage.IsSuccessStatusCode)
            //responseMessage.EnsureSuccessStatusCode();

            return responseMessage.Headers.Location;
        }


        static async Task<string> UpdateEmployeeAsync(Employee employee)
        {
            HttpResponseMessage responseMessage = await client.PutAsXmlAsync($"api/Employee/{employee.id}", employee);

            //if(responseMessage.IsSuccessStatusCode)
            //responseMessage.EnsureSuccessStatusCode();

            return responseMessage.Content.ReadAsStringAsync().Result;
        }

        static async Task<string> GetAllEmployees()
        {
            //Authorization authorization = new Authorization(tokenResult.access_token);
           
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",tokenResult.access_token);
            HttpResponseMessage responseMessage = await client.GetAsync($"api/Employee/");
            string str = responseMessage.Content.ReadAsStringAsync().Result;
            return str;
        }

        static async Task<string> GetAllEmployeeVersion2()
        {
            HttpResponseMessage responseMessage = await client.GetAsync($"api/2/EmployeeAll/");
            string str = responseMessage.Content.ReadAsStringAsync().Result;
            return str;
        }

        static async Task<string> GetEmployeeById(int id)
        {
            HttpResponseMessage responseMessage = await client.GetAsync($"api/Employee/{id}");

            //Employee employee = null;
            string str;

            //if (responseMessage.IsSuccessStatusCode)
            //{
            //employee = await responseMessage.Content.ReadAsAsync<Employee>();
            str = responseMessage.Content.ReadAsStringAsync().Result;
            //}

            // = responseMessage.Content.ReadAsStringAsync().Result;
            return str;
        }

        static async Task<List<Employee>> GetEmployeesAsync(string path)
        {
            List<Employee> employees = null;
            HttpResponseMessage response = await client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                employees = await response.Content.ReadAsAsync<List<Employee>>();
            }
            return employees;
        }

        static void ShowProduct(List<Employee> employees)
        {
            foreach (Employee employee in employees)
                Console.WriteLine($"Name: {employee.first_name}\tSalary: " +
                    $"{employee.salary}\tCity: {employee.city}");
        }

        static async Task RunAsync()
        {

            client.BaseAddress = new Uri("http://localhost:60390/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/xml"));

            try
            {
                string path = @"http://localhost:60390/api/Employee";
                List<Employee> employees = await GetEmployeesAsync(path);
                ShowProduct(employees);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }


    }
}
