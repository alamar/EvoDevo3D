// Появление организатора на боку
// 

resilience = 0.4;
if (numDivisions<10)
   {SpawnWherever();}
   else
   {
	if (EnvironmentalAccess>0.2 && getSensorReaction(0)==0)
	    {Spill(0);
		 color=1; }
	if (getSensorReaction(1)==0 && EnvironmentalAccess>0 && getSensorReaction(0)>0.148 && getSensorReaction(0)<0.149)
//	if (EnvironmentalAccess>0 && getSensorReaction(0)>0.148 && getSensorReaction(0)<0.149)
	    {Spill(1);
		color=2;}
   
   }




