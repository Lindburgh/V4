public void OnAnswerSelected(int index)
{    
    bool isCorrect = (index == currentQuestion.correctAnswerIndex);    
    CheckAnswer(isCorrect);
}
