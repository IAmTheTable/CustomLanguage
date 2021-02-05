using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
public class Config{
  public static char EndOfLine = ';';
  public static char define = '=';
  public static char CharCode = '\'';
  public static char StringCode = '"';
}
class MainClass {

  public static int currentLine = 0;
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
    if(input.ToString().Contains("\"")){
      return true;
    }
    else
      return false;
  }
    public static bool isBool(object input){
      if(input.ToString() == "true" || input.ToString() == "false"){
        return true;
      }
      else{
        return false;
      }
  }
    public static bool isInt(object input){
      try{
        input = int.Parse(input.ToString());
        return true;
      }
      catch(Exception){
        return false;
      }
      
    }
    public static bool isChar(object input){
      input = char.Parse(input.ToString());
    if(input.GetType() == typeof(char)){
      return true;
    }
    else
      return false;
  }

  public static Dictionary<Tuple<TokenType,TokenType,string>,string> data = new Dictionary<Tuple<TokenType,TokenType,string>,string>();

  public static Dictionary<Tuple<ExpressionType,string>,string[]> expressionData = new Dictionary<Tuple<ExpressionType,string>,string[]>();
  public static Tuple<bool,string> tryGetValueFromData(object input){
    
    //Console.WriteLine("got input " + input);
    try{
      var TupleInfo = Tuple.Create(TokenType.Any, TokenType.String, input.ToString());
      return Tuple.Create(true,(data[TupleInfo]));
      //Console.WriteLine(data[TupleInfo]);
    }catch(Exception){
      try{
        var TupleInfo = Tuple.Create(TokenType.Any, TokenType.Int, input.ToString());
        return Tuple.Create(true,(data[TupleInfo]));
        //Console.WriteLine("Info " + data[TupleInfo]);
      }catch(Exception){
        try{
            var TupleInfo = Tuple.Create(TokenType.Any, TokenType.Bool, input.ToString());
            return Tuple.Create(true,(data[TupleInfo]));            //Console.WriteLine(data[TupleInfo]);
        }
        catch(Exception){
          try{
            var TupleInfo = Tuple.Create(TokenType.Any, TokenType.Char, input.ToString());
            return Tuple.Create(true,(data[TupleInfo]));
            //Console.WriteLine(data[TupleInfo]);
          }
          catch(Exception){
            return Tuple.Create(false,"");
            //throw new Exception("Value not in data table.");
          }
        }
      }
    }
  }
public static bool Recompile(string input){
  try{
    foreach(var Semi in input.Split(Config.EndOfLine)){
    //Define the variables into the data table
      switch(Semi.Split(" ")[0]){
        case "string":
          var key = Semi.Split(Config.define)[1].Split(Config.EndOfLine)[0];
          if(key.StartsWith(" ")){
            key = key.Substring(1);
          }
          if(tryGetValueFromData(key).Item1 == true || key.StartsWith("\"") && key.EndsWith("\"")){
            //Console.WriteLine("Key pt " + key + " " + isString(key));
            if(isString(key)){
              var tupleData = Tuple.Create(TokenType.Any,TokenType.String,Semi.Split(" ")[1].Split(Config.define)[0]);
            //Console.WriteLine("key pt2 " + tupleData);
              data.Add(tupleData,key);
              break;
            }
            else{
            //Console.WriteLine("xd" + Semi.Split("string")[1].Split(" ")[1].Split(" ")[0]);
              var data2 = tryGetValueFromData(key);
            //Console.WriteLine("PreKek2" + key);
            //Console.WriteLine("KEK2" + data2);
              var TupleData = Tuple.Create(TokenType.Any,TokenType.String,Semi.Split("string")[1].Split(" ")[1].Split(" ")[0]);
              data.Add(TupleData, data2.Item2);
              break;
            }
          }
          else{
            throw new Exception("Exception thrown at line " + currentLine + " reason: string type invalid.");
          }
          
        case "int":
          var intkey = Semi.Split(Config.define)[1].Split(Config.EndOfLine)[0];
          if(intkey.StartsWith(" ")){
            intkey = intkey.Substring(1);
          }
          List<int> intContents = new List<int>();
          if(intkey.Split("+").Length > 1){
            foreach(var keyVal in intkey.Split("+")){
              var keyValFormat = keyVal;
              if(keyValFormat.StartsWith(" ")){
                keyValFormat = keyValFormat.Substring(1);
              }
              if(keyValFormat.EndsWith(" ")){
                keyValFormat = keyValFormat.Remove(keyValFormat.Length - 1);
              }
              if(tryGetValueFromData(keyValFormat).Item1 == true){
                intContents.Add(int.Parse(tryGetValueFromData(keyValFormat).Item2));
                //Console.WriteLine("INt test" + int.Parse(tryGetValueFromData(keyValFormat).Item2) + "f");
                //Console.WriteLine("tt" + Semi.Split("int ")[1].Split(Config.define)[0] + "tt");
              }
              else{
                intContents.Add(int.Parse(keyValFormat));
              }
            }
            int globalReturn = 0;
            foreach(int key2 in intContents){
              globalReturn = globalReturn + key2;
            }
            //Console.WriteLine("Got final int " + globalReturn + "f");
            var varName = Semi.Split("int ")[1].Split(Config.define)[0];
            if(varName.StartsWith(" ")){
              varName = varName.Substring(1);
            }
            if(varName.EndsWith(" ")){
              varName = varName.Remove(varName.Length - 1);
            }
            var TupleDataINt = Tuple.Create(TokenType.Any,TokenType.Int,varName);
            data.Add(TupleDataINt, globalReturn.ToString());
            break;
          }
          else
          {
            if(isInt(Semi.Split(Config.define)[1].Split(Config.EndOfLine)[0])){
              var TupleData = Tuple.Create(TokenType.Any, TokenType.Int, Semi.Split(" ")[1].Split(Config.define)[0]);
              data.Add(TupleData,intkey);
              break;
            }
            else{
              var TupleData = Tuple.Create(TokenType.Any, TokenType.Int, Semi.Split(" ")[1].Split(Config.define)[0]);
              //Console.WriteLine("trying " + key);
              try{
                if(tryGetValueFromData(intkey).Item1 == true){
                  data.Add(TupleData, tryGetValueFromData(intkey).Item2);
                }
                else{
                  throw new Exception("Variable not found or initialized.");
                }
              }
              catch(Exception){}
              break;
            }
            //Console.WriteLine("k"+key);

          }
        case "bool":
          var value = Semi.Split(Config.define)[1].Split(Config.EndOfLine)[0];
          if(value.StartsWith(" ")){
            value = value.Substring(1);
          }
          var varNameB = Semi.Split("bool ")[1].Split(" ")[0];
          if(isBool(value) == true){
            var TupleData = Tuple.Create(TokenType.Any, TokenType.Bool, varNameB);
            data.Add(TupleData,Semi.Split(Config.define)[1].Split(Config.EndOfLine)[0]);
            break;
          }
          else if(tryGetValueFromData(value).Item1 == true){
            var TupleData = Tuple.Create(TokenType.Any, TokenType.Bool, varNameB);
            data.Add(TupleData,tryGetValueFromData(value).Item2);
            break;
          }
          else{
            throw new Exception("Invalid type of bool at " + Semi.Split(" ")[1].Split(Config.define)[0] + " line: " + currentLine + " reason: " + value + " is either not defined or isnt of type 'bool'.");
          }
        case "char":
          /*var key = Semi.Split(Config.define)[1].Split(Config.EndOfLine)[0];
          if(key.StartsWith(" ")){
            key = key.Substring(1);
          }*/
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
        List<string> variables = new List<string>();
          var data = Semi.Split(Config.EndOfLine)[0].Split("(")[1].Split(")")[0];
          if(data.Split("+").Length > 1){
            foreach(var data2 in data.Split("+")){
              var temp = data2;
              //Console.WriteLine("PRECEHCK " + temp + " " + temp.Length);
              if(temp.StartsWith(" ")){
                temp = temp.Substring(1);
                //Console.WriteLine("Starts with a space");
              }
              if(temp.EndsWith(" ")){
                temp = temp.Remove(temp.Length - 1);
                //Console.WriteLine("ends with space");
              }
              if(temp.StartsWith("\"") && temp.EndsWith("\"")){
                variables.Add(temp.Split("\"")[1].Split("\"")[0]);
              }
              else if(temp.StartsWith("'") && temp.EndsWith("'")){
                variables.Add(temp.Split("'")[1].Split("'")[0]);
              }
              else{
                if(tryGetValueFromData(temp).Item1){
                  variables.Add(tryGetValueFromData(temp).Item2);
                }
                else{
                  throw new Exception("Variable " + temp + " not found or not defined at line " + currentLine);
                }
              }
            }
            List<string> finalData = new List<string>();
            foreach(var x in variables){
              var xVar = x;
              if(xVar.StartsWith("\"")){
                xVar = xVar.Substring(1);
              }
              if(xVar.EndsWith("\"")){
                xVar = xVar.Remove(xVar.Length - 1);
              }
              finalData.Add(xVar);
            }
            var TupleData = Tuple.Create(ExpressionType.Print,Semi.Split("Print")[1].Split(Config.EndOfLine)[0]);
            var extraArgs = finalData.ToArray();
            expressionData.Add(TupleData,extraArgs);
            break;
          }
          else{
            if(tryGetValueFromData(data).Item1 == true){
              var TupleData = Tuple.Create(ExpressionType.Print,Semi.Split("Print")[1].Split(Config.EndOfLine)[0]);
              var extraArgs = new string[1];
              //Console.WriteLine("data " + data);
              extraArgs[0] = tryGetValueFromData(data).Item2.Split("\"")[1].Split("\"")[0];
              expressionData.Add(TupleData,extraArgs);
              break;
            }
            else{
              var TupleData = Tuple.Create(ExpressionType.Print,Semi.Split("Print")[1].Split(Config.EndOfLine)[0]);
              var extraArgs = new string[1];
              //Console.WriteLine("data " + data);
              extraArgs[0] = data;
              expressionData.Add(TupleData,extraArgs);
              break;
            }
          }
        default:
          break;
      }
    }
    return true;
  }
  catch(Exception exx){
    Console.WriteLine(exx.StackTrace + "\n" + exx.Message);
    return true;
  }
}
public static bool ExecuteExpression(){
  foreach(var exp in expressionData.Keys){
    switch(exp.Item1){
      case ExpressionType.Print:
        string final = "";
        foreach(var variable in expressionData[exp]){
         final = final + variable;
        }
        if(final.StartsWith("\"")){
          final = final.Substring(1);  
        }
        if(final.EndsWith("\"")){
          final = final.Remove(final.Length - 1);
        }
        Console.WriteLine(final);
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
      currentLine++;
      Recompile(s);
    }
    foreach(var item in data.Keys){
      //Console.WriteLine(item + " - " + data[item]);
    }
    foreach(var item in expressionData.Keys){
      //Console.WriteLine(item + " - " + expressionData[item]);
    } 
    ExecuteExpression();
  }
}