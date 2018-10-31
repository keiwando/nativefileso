//
//  EPPZSwizzler.m
//  eppz!swizzler
//
//  Created by Borbás Geri on 27/02/14
//  Copyright (c) 2013 eppz! development, LLC.
//
//  follow http://www.twitter.com/_eppz
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#import "EPPZSwizzler.h"


#define log(...) [self log:[NSString stringWithFormat:__VA_ARGS__]]
#define error(...) [self error:[NSString stringWithFormat:__VA_ARGS__]]


static BOOL _eppzSwizzlerLogging;
static NSString *_eppzSwizzlerLatestErrorMessage;


static char associationKeyKey;


@interface EPPZSwizzler ()
+(NSString*)setterMethodNameForPropertyName:(NSString*) propertyName;
@end


@implementation EPPZSwizzler



#pragma mark - Logging

+(void)setLogging:(BOOL) isOn
{ _eppzSwizzlerLogging = isOn; }

+(void)log:(NSString*) message
{
    if (_eppzSwizzlerLogging == NO) return; // Checks
    NSLog(@"%@", message); // Show
}

+(void)error:(NSString*) errorMessage
{
    _eppzSwizzlerLatestErrorMessage = errorMessage; // Reference
    
    if (_eppzSwizzlerLogging == NO) return; // Checks
    NSLog(@"%@", errorMessage); // Show
}

+(NSString*)latestErrorMessage
{ return _eppzSwizzlerLatestErrorMessage; }


#pragma mark - Method swizzlers

+(void)swapInstanceMethod:(SEL) oneSelector
       withInstanceMethod:(SEL) otherSelector
                  ofClass:(Class) class
{
    // Get methods.
    Method oneMethod = class_getInstanceMethod(class, oneSelector);
    Method otherMethod = class_getInstanceMethod(class, otherSelector);
    
    // Checks.
    if (oneMethod == nil)
    { error(@"Instance method `%@` not found on class %@", NSStringFromSelector(oneSelector), class); return; };
    if (otherMethod == nil)
    { error(@"Instance method `%@` not found on class %@", NSStringFromSelector(otherSelector), class); return; };
    
    // Exchange.
    method_exchangeImplementations(oneMethod, otherMethod);
    
    log(@"Swapped instance method `%@` with `%@` in %@", NSStringFromSelector(oneSelector), NSStringFromSelector(otherSelector), class);
}

+(void)swapClassMethod:(SEL) oneSelector
       withClassMethod:(SEL) otherSelector
               ofClass:(Class) class
{
    // Get methods.
    Method oneMethod = class_getClassMethod(class, oneSelector);
    Method otherMethod = class_getClassMethod(class, otherSelector);
    
    // Checks.
    if (oneMethod == nil)
    { error(@"Class method `%@` not found on class %@", NSStringFromSelector(oneSelector), class); return; };
    if (otherMethod == nil)
    { error(@"Class method `%@` not found on class %@", NSStringFromSelector(otherSelector), class); return; };
    
    // Exchange.
    method_exchangeImplementations(oneMethod, otherMethod);
    
    log(@"Swapped class method `%@` with `%@` in %@", NSStringFromSelector(oneSelector), NSStringFromSelector(otherSelector), class);
}

+(void)replaceClassMethod:(SEL) selector
                  ofClass:(Class) targetClass
                fromClass:(Class) sourceClass
{
    // Get methods.
    Method targetMethod = class_getClassMethod(targetClass, selector);
    Method sourceMethod = class_getClassMethod(sourceClass, selector);
    
    // Checks.
    if (sourceMethod == nil)
    { error(@"Class method `%@` not found on source class %@", NSStringFromSelector(selector), sourceClass); return; };
    
    if (targetMethod == nil)
    { error(@"Class method `%@` not found on target class %@", NSStringFromSelector(selector), targetClass); return; };
    
    // Replace target method.
    IMP previousTargetMethod = method_setImplementation(targetMethod,
                                                        method_getImplementation(sourceMethod));
    
    log(@"Replaced method `%@` of %@ from %@ with %@", NSStringFromSelector(selector), targetClass, sourceClass, (previousTargetMethod) ? @"success" : @"error");
}

+(void)replaceInstanceMethod:(SEL) selector
                     ofClass:(Class) targetClass
                   fromClass:(Class) sourceClass
{
    // Get methods.
    Method targetMethod = class_getInstanceMethod(targetClass, selector);
    Method sourceMethod = class_getInstanceMethod(sourceClass, selector);
    
    // Checks.
    if (sourceMethod == nil)
    { error(@"Instance method `%@` not found on source class %@", NSStringFromSelector(selector), sourceClass); return; };
    
    if (targetMethod == nil)
    { error(@"Instance method `%@` not found on target class %@", NSStringFromSelector(selector), targetClass); return; };
    
    // Replace target method.
    IMP previousTargetMethod = method_setImplementation(targetMethod,
                                                        method_getImplementation(sourceMethod));
    
    log(@"Replaced instance method `%@` of %@ from %@ with %@", NSStringFromSelector(selector), targetClass, sourceClass, (previousTargetMethod) ? @"success" : @"error");
}

+(void)addClassMethod:(SEL) selector
              toClass:(Class) targetClass
            fromClass:(Class) sourceClass
{
    
    // Get methods.
    Method method = class_getClassMethod(sourceClass, selector);
    
    // Checks.
    if (method == nil)
    { error(@"Class method `%@` not found on source class %@", NSStringFromSelector(selector), sourceClass); return; };
    
    targetClass = object_getClass((id)targetClass);
    BOOL success = class_addMethod(targetClass,
                                   selector,
                                   method_getImplementation(method),
                                   method_getTypeEncoding(method));
    
    log(@"Added class method `%@` of %@ to %@ with %@", NSStringFromSelector(selector), sourceClass, targetClass, (success) ? @"success" : @"error");
}

+(void)addInstanceMethod:(SEL) selector
                 toClass:(Class) targetClass
               fromClass:(Class) sourceClass
{
    [self addInstanceMethod:selector
                    toClass:targetClass
                  fromClass:sourceClass
                         as:selector];
}

+(void)addInstanceMethod:(SEL) selector
                 toClass:(Class) targetClass
               fromClass:(Class) sourceClass
                      as:(SEL) targetSelector
{
    // Get method.
    Method method = class_getInstanceMethod(sourceClass, selector);
    
    // Checks.
    if (method == nil)
    { error(@"Instance method `%@` not found on source class %@", NSStringFromSelector(selector), sourceClass); return; };
    
    // Add method.
    BOOL success = class_addMethod(targetClass,
                                   targetSelector,
                                   method_getImplementation(method),
                                   method_getTypeEncoding(method));
    
    log(@"Added instance method `%@` of %@ to %@ with %@", NSStringFromSelector(selector), sourceClass, targetClass, (success) ? @"success" : @"error");
}


#pragma mark - Property swizzlers

+(NSString*)setterMethodNameForPropertyName:(NSString*) propertyName
{
    // Checks.
    if (propertyName.length == 0) return propertyName;
    
    NSString *firstChar = [[propertyName substringToIndex:1] capitalizedString];
    NSString *andTheRest = [propertyName substringFromIndex:1];
    return [NSString stringWithFormat:@"set%@%@:", firstChar, andTheRest];
}

+(void)addPropertyNamed:(NSString*) propertyName
                toClass:(Class) targetClass
              fromClass:(Class) sourceClass
{
    // Get property.
    const char *name = propertyName.UTF8String;
    objc_property_t property = class_getProperty(sourceClass, name);
    unsigned int attributesCount = 0;
    objc_property_attribute_t *attributes = property_copyAttributeList(property, &attributesCount);
    
    // Checks.
    if (property == nil)
    { error(@"Property `%@` not found on source class %@", propertyName, sourceClass); return; };
    
    // Add (or replace) property.
    BOOL success = class_addProperty(targetClass, name, attributes, attributesCount);
    if (success == NO)
    {
        class_replaceProperty(targetClass, name, attributes, attributesCount);
        log(@"Replaced property `%@` of %@ from %@ with %@", propertyName, targetClass, sourceClass, (success) ? @"success" : @"error");
    }
    else
    { log(@"Added property `%@` to %@ from %@ with %@", propertyName, targetClass, sourceClass, (success) ? @"success" : @"error"); }
    
    // Add getter.
    [self addInstanceMethod:NSSelectorFromString(propertyName) toClass:targetClass fromClass:sourceClass];
    
    // Add setter.
    NSString *setterMethodName = [self setterMethodNameForPropertyName:propertyName];
    [self addInstanceMethod:NSSelectorFromString(setterMethodName) toClass:targetClass fromClass:sourceClass];
}

+(void)synthesizePropertyNamed:(NSString*) propertyName
                        ofKind:(Class) kind
                      forClass:(Class) targetClass
                    withPolicy:(EPPZSwizzlerProperryAssociationPolicy) policy
{
    // Get type encoding.
    const char *typeEncoding = @encode(typeof(kind));
    
    // Associate the key for the property to the class itself.
    NSString *keyObject = [NSString stringWithFormat:@"%@Key", propertyName];
    void *key = (__bridge void*)keyObject;
    objc_setAssociatedObject(targetClass, &associationKeyKey, keyObject, OBJC_ASSOCIATION_RETAIN_NONATOMIC);
    
    // Getter implementation.
    IMP getterImplementation = imp_implementationWithBlock(^(id self)
                                                           { return (id)objc_getAssociatedObject(self, key); });
    
    // Setter implementation.
    IMP setterImplementation = imp_implementationWithBlock(^(id self, id value)
                                                           { objc_setAssociatedObject(self, key, value, policy); });
    
    // Add getter.
    BOOL success = class_addMethod(targetClass,
                                   NSSelectorFromString(propertyName),
                                   getterImplementation,
                                   typeEncoding);
    
    log(@"Added synthesized getter `%@` to %@ with %@", propertyName, targetClass, (success) ? @"success" : @"error");
    
    // Add setter.
    NSString *setterMethodName = [self setterMethodNameForPropertyName:propertyName];
    success = class_addMethod(targetClass,
                              NSSelectorFromString(setterMethodName),
                              setterImplementation,
                              typeEncoding);
    
    log(@"Added synthesized setter `%@` to %@ with %@", setterMethodName, targetClass, (success) ? @"success" : @"error");
}



@end
