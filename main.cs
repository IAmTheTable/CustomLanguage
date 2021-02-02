using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
public class Config{
  public static char EndOfLine = ';';
  public static char define = '=';
  public static char CharCode = '\'';
  public static char StringCode = '"';
}
class MainClass {

  public enum TokenType{
    Any,
    String,
    Int,
    Bool,
    Char
  }
  public enum ExpressionType{
    Print,
    Add,
    Subtract
  }

  public static bool isString(object input){
    if(input.GetType() == typeof(string)){
      return true;
    }
    else
      return false;
  }
    public static bool isBool(string input){
    if(input == "true" || input == "True" || input == "False" || input == "false"){
      return true;
    }
    else
      return false;
  }
    public static bool isInt(object input){
    if(input.GetType() == typeof(int)){
      return true;
    }
    else
      return false;
  }
    public static bool isChar(object input){
    if(input.GetType() == typeof(char)){
      return true;
    }
    else
      return false;
  }

  public static Dictionary<Tuple<TokenType,TokenType,string>,string> data = new Dictionary<Tuple<TokenType,TokenType,string>,string>();

  public static Dictionary<Tuple<ExpressionType,string>,string[]> expressionData = new Dictionary<Tuple<ExpressionType,string>,string[]>();
  public static Tuple<bool,object> tryGetValueFromData(string input){
    try{
      var TupleInfo = Tuple.Create(TokenType.Any, TokenType.String, input);
      return Tuple.Create(true,(object)(data[TupleInfo]));
      //Console.WriteLine(data[TupleInfo]);
    }catch(Exception){
      try{
        var TupleInfo = Tuple.Create(TokenType.Any, TokenType.Int, input);
        return Tuple.Create(true,(object)(data[TupleInfo]));
        //Console.WriteLine(data[TupleInfo]);
      }catch(Exception){
        try{
            var TupleInfo = Tuple.Create(TokenType.Any, TokenType.Bool, input);
            return Tuple.Create(true,(object)(data[TupleInfo]));            //Console.WriteLine(data[TupleInfo]);
        }
        catch(Exception){
          try{
            var TupleInfo = Tuple.Create(TokenType.Any, TokenType.Char, input);
            return Tuple.Create(true,(object)(data[TupleInfo]));
            //Console.WriteLine(data[TupleInfo]);
          }
          catch(Exception){
            return Tuple.Create(false,new object());
            //throw new Exception("Value not in data table.");
          }
        }
      }
    }
  }
public static bool Recompile(string input){
  foreach(var Semi in input.Split(Config.EndOfLine)){
    //Define the variables into the data table
      switch(Semi.Split(" ")[0]){
        case "string":
          if(isString(Semi.Split("\"")[1].Split("\"")[0])){
            var TupleData = Tuple.Create(TokenType.Any, TokenType.String, Semi.Split(" ")[1].Split(Config.define)[0]);
            data.Add(TupleData,Semi.Split("\"")[1].Split("\"")[0]);
            //Console.WriteLine("string found");
            break;
          }
          else{
            throw new Exception("LOL STRING");
          }
        case "int":
        if(tryGetValueFromData(Semi.Split(Config.define)[1].Split(Config.EndOfLine)[0]).Item1 == true){
          var TupleData = Tuple.Create(TokenType.Any, TokenType.Int, Semi.Split(" ")[1].Split(Config.define)[0]);
          data.Add(TupleData,tryGetValueFromData(Semi.Split(Config.define)[1].Split(Config.EndOfLine)[0]).Item2.ToString());
        }
        else if(isInt(int.Parse(Semi.Split(Config.define)[1].Split(Config.EndOfLine)[0]))){
            var TupleData = Tuple.Create(TokenType.Any, TokenType.Int, Semi.Split(" ")[1].Split(Config.define)[0]);
            data.Add(TupleData,Semi.Split(Config.define)[1].Split(Config.EndOfLine)[0];
            //Console.WriteLine("number found");
            break;
          }
          else{
            throw new Exception(Semi.Split(Config.define)[1].Split(Config.EndOfLine)[0] + " LOL INT");
          }
        case "bool":
          if(isBool(Semi.Split(Config.define)[1].Split(Config.EndOfLine)[0]) == false){
            var TupleData = Tuple.Create(TokenType.Any, TokenType.Bool, Semi.Split(" ")[1].Split(Config.define)[0]);
            data.Add(TupleData,Semi.Split(Config.define)[1].Split(Config.EndOfLine)[0]);
            break;
          }
          else{
            throw new Exception(isBool(Semi.Split(Config.define)[1].Split(Config.EndOfLine)[0]) + " LOL BOOL");
          }
        case "char":
          if(Semi.Split("=")[1].Contains("'")){
            if(Semi.Split("'")[1].Split("'")[0].Length != 1){
              throw new Exception("LOL CHAR");
            }
            else{
              var TupleData = Tuple.Create(TokenType.Any, TokenType.Char, Semi.Split(" ")[1].Split(Config.define)[0]);
              data.Add(TupleData,Semi.Split(Config.define)[1].Split(Config.EndOfLine)[0]);
              break;
            }
          }
          else{
            throw new Exception("LOL CHAR");
          }
      }

      //Run the expressions
      switch(Semi.Split(Config.EndOfLine)[0].Split("(")[0]){
        case "Print":
          var TupleData = Tuple.Create(ExpressionType.Print,Semi.Split("Print")[1].Split(Config.EndOfLine)[0]);
          var extraArgs = new string[1];
          expressionData.Add(TupleData,extraArgs);
          break;
        default:
          break;
      }
  }
  return true;
}
public static bool ExecuteExpression(){
  foreach(var exp in expressionData.Keys){
    //Console.WriteLine("Item val " + exp.Item2);
    switch(exp.Item1){
      case ExpressionType.Print:
        if(exp.Item2.Contains("\"")){
          Console.WriteLine(exp.Item2.Split("\"")[1].Split("\"")[0]);
        }
        else if(!exp.Item2.Contains("\"") || !exp.Item2.Contains("'")){
          if(tryGetValueFromData(exp.Item2.Split("(")[1].Split(")")[0].ToString()).Item1 == true){
            Console.WriteLine(tryGetValueFromData(exp.Item2.Split("(")[1].Split(")")[0].ToString()).Item2);
          }
          //Console.WriteLine(lol);
          /*try{
            Console.WriteLine(data[Tuple.Create(TokenType.Any,TokenType.Bool,lol)]);
          }
          catch(Exception e){
            try{
              Console.WriteLine(data[Tuple.Create(TokenType.Any,TokenType.Int,lol)]);
            }
            catch(Exception ee){
              try{
                Console.WriteLine(data[Tuple.Create(TokenType.Any,TokenType.String,lol)]);
              }catch(Exception eee){
                try{
                  Console.WriteLine(data[Tuple.Create(TokenType.Any,TokenType.Char,lol)]);
                }catch(Exception eeee){
                  throw new Exception("Variable " + lol + " not found.");
                }
              }
            }
          }*/
        }
        break;
      default: 
        break;
    }
  }
  return true;
}

  public static void Main (string[] args) {
    string[] input = File.ReadAllLines("main.TFG");
    foreach(var s in input){
      //Console.WriteLine(s);
      Recompile(s);
    }
    ExecuteExpression();
  }
}